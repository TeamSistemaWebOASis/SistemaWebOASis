using GestorErrores;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Web;

namespace SitioWebOasis.CommonClasses
{
	/// <summary>
	/// Permite la configuración del Caché de ASP.Net mediante
	/// la sección CacheSettings del archivo Web.config.
	/// </summary>
	public enum CacheExpirationType {Absolute, Sliding};

	public class CacheConfig
	{
		protected static string cacheSettingsConfigName = "CacheSettings";
		protected static string EnableCachingKey = "EnableCaching";
		protected static string RequireKeyDefinitionInConfigKey = "RequireKeyDefinitionInConfig";
		protected static string DefaultCacheDurationUnitsKey = "DefaultCacheDurationUnits";
		protected static string DefaultCacheDurationValueKey = "DefaultCacheDurationValue";
		protected static NameValueCollection config;
		protected static System.Web.HttpContext context;

		public static readonly bool RequireKeyDefinitionInConfig;
		public static readonly bool EnableCaching;
		public static readonly TimeSpan DefaultCacheDuration;

		static CacheConfig()
		{
			#region Load Configuration Section
			try
			{
                config = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection(cacheSettingsConfigName);
			}
			catch (Exception ex)
			{
                Errores err = new Errores();
                err.SetError(   ex,
                                "Configuration section " + cacheSettingsConfigName + " is not defined correctly in web.config.");
			}
			#endregion

			#region Load EnableCaching Flag
			try
			{
				EnableCaching = bool.Parse(config[EnableCachingKey]);
			}
			catch (Exception ex)
			{
                Errores err = new Errores();
                err.SetError(   ex,
                                EnableCachingKey + " is not defined as either 'true' or 'false' in configuration section " + cacheSettingsConfigName);
			}
			#endregion

			#region Load RequireKeyDefinition flag
			// load RequireKeyDefinitionInConfig flag
			try
			{
				RequireKeyDefinitionInConfig = bool.Parse(config[RequireKeyDefinitionInConfigKey]);
			}
			catch (Exception ex)
			{                
                throw new System.Configuration.ConfigurationErrorsException(RequireKeyDefinitionInConfigKey + " is not defined as either 'true' or 'false' in configuration section " + cacheSettingsConfigName, ex);
			}
			#endregion

			#region Load DefaultCacheDuration
			try
			{
				DefaultCacheDuration = GetTimeSpanFromUnits(config[DefaultCacheDurationUnitsKey],
					Int32.Parse(config[DefaultCacheDurationValueKey]));
			}
			catch (ConfigurationException ex)
			{
				throw ex;
			}
			catch (Exception ex)
			{
                throw new System.Configuration.ConfigurationErrorsException("Invalid or missing value for " + DefaultCacheDurationUnitsKey + " in configuration.", ex);
            }
			#endregion

			#region Check HttpContext Available
			context = System.Web.HttpContext.Current;
			if(context == null)
				EnableCaching = false;
			#endregion
		}

		#region Private Helper Methods: GetTimeSpanFromUnits, VerifyCacheDuration, GetCacheDuration, ArgsToString
		private static TimeSpan GetTimeSpanFromUnits(string units, int value)
		{
			switch (units)
			{
				case "seconds" : 
					return TimeSpan.FromSeconds(value);
				case "minutes" :
					return TimeSpan.FromMinutes(value);
				case "hours" :
					return TimeSpan.FromHours(value);
				default :
					throw new InvalidCastException("Invalid time span units: " + units);
			}
		}

		private static int SearchKey(string key)
		{
      string[] Keys = config.AllKeys;
			for (int i=0; i<Keys.Length; i++)
			{
				if (key.StartsWith(Keys[i]))
					return i;
			}
			return -1;
		}

		private static void VerifyCacheDuration(string key)
		{
			if((SearchKey(key) < 0) && (RequireKeyDefinitionInConfig)){
                throw new System.Configuration.ConfigurationErrorsException(key + " is not defined in config section " + cacheSettingsConfigName + "." );
            }
				
		}
		private static TimeSpan GetCacheDuration(string key)
		{
			VerifyCacheDuration(key);
			int index = SearchKey(key);
			object val = config[index];
			if(val == null)
			{
				return DefaultCacheDuration;
			}
			else
			{
				try
				{
					return GetTimeSpanFromUnits(config[DefaultCacheDurationUnitsKey],
						Int32.Parse(val.ToString()));
				}
				catch (Exception ex)
				{
					if(val.ToString().ToLower() == "default"){
                        return DefaultCacheDuration;
                    }

                    throw new System.Configuration.ConfigurationErrorsException("Invalid value for configuration key " + key + ".", ex);
                }
			}
		}

		#endregion

		#region EnCache
		public static void Insert(string key, object value)
		{
			Insert(key, value, GetCacheDuration(key), CacheExpirationType.Absolute);
		}

		public static void Insert(string key, object value, CacheExpirationType ExType,  System.Web.Caching.CacheItemPriority priority, System.Web.Caching.CacheItemRemovedCallback callback)
		{
			Insert(key,value,GetCacheDuration(key),ExType,priority,callback);
		}

		public static void Insert(string key, object value, TimeSpan cacheDuration, CacheExpirationType ExType)
		{
			if(!EnableCaching)
			{
				context.Trace.Warn("Caching is disabled.");
			}
			else
			{
				context = System.Web.HttpContext.Current;
				switch (ExType)
				{
					case CacheExpirationType.Absolute:
						context.Cache.Insert(key, value, null, DateTime.Now.Add(cacheDuration), System.Web.Caching.Cache.NoSlidingExpiration);
						break;
					case CacheExpirationType.Sliding:
						context.Cache.Insert(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, cacheDuration);
						break;
				}
				
				context.Trace.Write(key + " cached for a duration of " + cacheDuration + ".");
			}
		}

		public static void Insert(string key, object value, TimeSpan cacheDuration, CacheExpirationType ExType, System.Web.Caching.CacheItemPriority priority, System.Web.Caching.CacheItemRemovedCallback callback)
		{
			if(!EnableCaching)
			{
				context.Trace.Warn("Caching is disabled.");
			}
			else
			{
				context = System.Web.HttpContext.Current;
				switch (ExType)
				{
					case CacheExpirationType.Absolute:
						context.Cache.Insert(key, value, null, DateTime.Now.Add(cacheDuration), System.Web.Caching.Cache.NoSlidingExpiration,priority,callback);
						break;
					case CacheExpirationType.Sliding:
						context.Cache.Insert(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, cacheDuration, priority,callback);
						break;
				}
				
				context.Trace.Write(key + " cached for a duration of " + cacheDuration + ".");
			}
		}


		#endregion

		#region DeCache
		public static object Get(string key)
		{
			object cacheItem = null;

			if(EnableCaching)
			{
				VerifyCacheDuration(key);
				cacheItem = context.Cache[key];
			}
			return cacheItem;
		}

		public static void Remove(string key)
		{
			if(EnableCaching)
			{
				VerifyCacheDuration(key);
				context.Cache.Remove(key);
			}
		}
		#endregion

	}
}

