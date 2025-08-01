//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v14.4.0.0 (NJsonSchema v11.3.2.0 (Newtonsoft.Json v13.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using HomeScreen.Service.Weather.Generated.Entities;

#pragma warning disable 108 // Disable "CS0108 '{derivedDto}.ToJson()' hides inherited member '{dtoBase}.ToJson()'. Use the new keyword if hiding was intended."
#pragma warning disable 114 // Disable "CS0114 '{derivedDto}.RaisePropertyChanged(String)' hides inherited member 'dtoBase.RaisePropertyChanged(String)'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword."
#pragma warning disable 472 // Disable "CS0472 The result of the expression is always 'false' since a value of type 'Int32' is never equal to 'null' of type 'Int32?'
#pragma warning disable 612 // Disable "CS0612 '...' is obsolete"
#pragma warning disable 649 // Disable "CS0649 Field is never assigned to, and will always have its default value null"
#pragma warning disable 1573 // Disable "CS1573 Parameter '...' has no matching param tag in the XML comment for ...
#pragma warning disable 1591 // Disable "CS1591 Missing XML comment for publicly visible type or member ..."
#pragma warning disable 8073 // Disable "CS8073 The result of the expression is always 'false' since a value of type 'T' is never equal to 'null' of type 'T?'"
#pragma warning disable 3016 // Disable "CS3016 Arrays as attribute arguments is not CLS-compliant"
#pragma warning disable 8600 // Disable "CS8600 Converting null literal or possible null value to non-nullable type"
#pragma warning disable 8602 // Disable "CS8602 Dereference of a possibly null reference"
#pragma warning disable 8603 // Disable "CS8603 Possible null reference return"
#pragma warning disable 8604 // Disable "CS8604 Possible null reference argument for parameter"
#pragma warning disable 8625 // Disable "CS8625 Cannot convert null literal to non-nullable reference type"
#pragma warning disable 8765 // Disable "CS8765 Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes)."

namespace HomeScreen.Service.Weather.Generated.Clients
{
    using System = global::System;

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "14.4.0.0 (NJsonSchema v11.3.2.0 (Newtonsoft.Json v13.0.0.0))")]
    public partial class OpenMeteoClient : IOpenMeteoClient
    {
        #pragma warning disable 8618
        private string _baseUrl;
        #pragma warning restore 8618

        private HttpClient _httpClient;
        private static System.Lazy<System.Text.Json.JsonSerializerOptions> _settings = new System.Lazy<System.Text.Json.JsonSerializerOptions>(CreateSerializerSettings, true);
        private System.Text.Json.JsonSerializerOptions _instanceSettings;

    #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public OpenMeteoClient(HttpClient httpClient)
    #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            BaseUrl = "https://api.open-meteo.com";
            _httpClient = httpClient;
            Initialize();
        }

        private static System.Text.Json.JsonSerializerOptions CreateSerializerSettings()
        {
            var settings = new System.Text.Json.JsonSerializerOptions();
            UpdateJsonSerializerSettings(settings);
            return settings;
        }

        public string BaseUrl
        {
            get { return _baseUrl; }
            set
            {
                _baseUrl = value;
                if (!string.IsNullOrEmpty(_baseUrl) && !_baseUrl.EndsWith("/"))
                    _baseUrl += '/';
            }
        }

        public System.Text.Json.JsonSerializerOptions JsonSerializerSettings { get { return _instanceSettings ?? _settings.Value; } }

        static partial void UpdateJsonSerializerSettings(System.Text.Json.JsonSerializerOptions settings);

        partial void Initialize();

        partial void PrepareRequest(HttpClient client, System.Net.Http.HttpRequestMessage request, string url);
        partial void PrepareRequest(HttpClient client, System.Net.Http.HttpRequestMessage request, System.Text.StringBuilder urlBuilder);
        partial void ProcessResponse(HttpClient client, System.Net.Http.HttpResponseMessage response);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <remarks>
        /// Retrieve the forecast
        /// </remarks>
        /// <param name="latitude">Geographical WGS84 coordinates of the location. Multiple coordinates are not supported</param>
        /// <param name="longitude">Geographical WGS84 coordinates of the location. Multiple coordinates are not supported</param>
        /// <param name="elevation">The elevation used for statistical downscaling. Per default, a 90 meter digital elevation model is used. You can manually set the elevation to correctly match mountain peaks. If &amp;elevation=nan is specified, downscaling will be disabled and the API uses the average grid-cell height</param>
        /// <param name="hourly">A list of weather variables which should be returned. Values can be comma separated, or multiple &amp;hourly= parameter in the URL can be used.</param>
        /// <param name="daily">A list of daily weather variable aggregations which should be returned. Values can be comma separated, or multiple &amp;daily= parameter in the URL can be used. If daily weather variables are specified, parameter timezone is required.</param>
        /// <param name="current">A list of weather variables to get current conditions.</param>
        /// <param name="temperature_unit">If `fahrenheit` is set, all temperature values are converted to Fahrenheit.</param>
        /// <param name="wind_speed_unit">Other wind speed speed units: `ms`, `mph` and `kn`</param>
        /// <param name="precipitation_unit">Other precipitation amount units: `inch`</param>
        /// <param name="timeformat">If format `unixtime` is selected, all time values are returned in UNIX epoch time in seconds. Please note that all timestamp are in GMT+0! For daily values with unix timestamps, please apply `utc_offset_seconds` again to get the correct date.</param>
        /// <param name="timezone">If timezone is set, all timestamps are returned as local-time and data is returned starting at 00:00 local-time. Any time zone name from the time zone database is supported. If auto is set as a time zone, the coordinates will be automatically resolved to the local time zone. For multiple coordinates, a comma separated list of timezones can be specified.</param>
        /// <param name="past_days">If `past_days` is set, yesterday or the day before yesterday data are also returned.</param>
        /// <param name="forecast_days">Per default, only 7 days are returned. Up to 16 days of forecast are possible.</param>
        /// <param name="forecast_hours">Similar to `forecast_days`, the number of timesteps of hourly data controlled. Instead of using the current day as a reference, the current hour time-step is used.</param>
        /// <param name="forecast_minutely_15">Similar to `forecast_days`, the number of timesteps of 15-minutely data controlled. Instead of using the current day as a reference, the current 15-minutely time-step is used.</param>
        /// <param name="past_hours">Similar to `forecast_days`, the number of timesteps of hourly data controlled. Instead of using the current day as a reference, the current hour time-step is used.</param>
        /// <param name="past_minutely_15">Similar to `forecast_days`, the number of timesteps of 15-minutely data controlled. Instead of using the current day as a reference, the current 15-minutely time-step is used.</param>
        /// <param name="start_date">The time interval to get weather data. A day must be specified as an ISO8601 date (e.g. 2022-06-30).</param>
        /// <param name="end_date">The time interval to get weather data. A day must be specified as an ISO8601 date (e.g. 2022-06-30).</param>
        /// <param name="start_hour">The time interval to get weather data for hourly data. Time must be specified as an ISO8601 date (e.g. 2022-06-30T12:00).</param>
        /// <param name="end_hour">The time interval to get weather data for hourly data. Time must be specified as an ISO8601 date (e.g. 2022-06-30T12:00).</param>
        /// <param name="start_minutely_15">The time interval to get weather data for 15 minutely data. Time must be specified as an ISO8601 date (e.g. 2022-06-30T12:00).</param>
        /// <param name="end_minutely_15">The time interval to get weather data for 15 minutely data. Time must be specified as an ISO8601 date (e.g. 2022-06-30T12:00).</param>
        /// <param name="models">Manually select one or more weather models. Per default, the best suitable weather models will be combined.</param>
        /// <returns>Successfully retrieve the current forecast</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        public virtual async System.Threading.Tasks.Task<SwaggerResponse<WeatherForecastResponse>> ForecastAsync(float latitude, float longitude, float? elevation = null, IEnumerable<HourlyParameter>? hourly = null, IEnumerable<DailyParameter>? daily = null, IEnumerable<CurrentParameter>? current = null, TemperatureUnit? temperature_unit = null, string? wind_speed_unit = null, DistanceUnit? precipitation_unit = null, TimeFormat? timeformat = null, string? timezone = null, int? past_days = null, int? forecast_days = null, int? forecast_hours = null, int? forecast_minutely_15 = null, int? past_hours = null, int? past_minutely_15 = null, DateOnly? start_date = null, DateOnly? end_date = null, DateTimeOffset? start_hour = null, DateTimeOffset? end_hour = null, DateTimeOffset? start_minutely_15 = null, DateTimeOffset? end_minutely_15 = null, IEnumerable<WeatherModels>? models = null, CellSelection? cell_selection = null, string? apikey = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken))
        {
            if (latitude == null)
                throw new System.ArgumentNullException("latitude");

            if (longitude == null)
                throw new System.ArgumentNullException("longitude");

            var client_ = _httpClient;
            var disposeClient_ = false;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));

                    var urlBuilder_ = new System.Text.StringBuilder();
                    if (!string.IsNullOrEmpty(_baseUrl)) urlBuilder_.Append(_baseUrl);
                    // Operation Path: "v1/forecast"
                    urlBuilder_.Append("v1/forecast");
                    urlBuilder_.Append('?');
                    urlBuilder_.Append(System.Uri.EscapeDataString("latitude")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(latitude, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                    urlBuilder_.Append(System.Uri.EscapeDataString("longitude")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(longitude, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                    if (elevation != null)
                    {
                        urlBuilder_.Append(System.Uri.EscapeDataString("elevation")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(elevation, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                    }
                    if (hourly != null)
                    {
                            foreach (var item_ in hourly) { urlBuilder_.Append(System.Uri.EscapeDataString("hourly")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(item_, System.Globalization.CultureInfo.InvariantCulture))).Append('&'); }
                    }
                    if (daily != null)
                    {
                            foreach (var item_ in daily) { urlBuilder_.Append(System.Uri.EscapeDataString("daily")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(item_, System.Globalization.CultureInfo.InvariantCulture))).Append('&'); }
                    }
                    if (current != null)
                    {
                            foreach (var item_ in current) { urlBuilder_.Append(System.Uri.EscapeDataString("current")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(item_, System.Globalization.CultureInfo.InvariantCulture))).Append('&'); }
                    }
                    if (temperature_unit != null)
                    {
                        urlBuilder_.Append(System.Uri.EscapeDataString("temperature_unit")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(temperature_unit, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                    }
                    if (wind_speed_unit != null)
                    {
                        urlBuilder_.Append(System.Uri.EscapeDataString("wind_speed_unit")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(wind_speed_unit, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                    }
                    if (precipitation_unit != null)
                    {
                        urlBuilder_.Append(System.Uri.EscapeDataString("precipitation_unit")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(precipitation_unit, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                    }
                    if (timeformat != null)
                    {
                        urlBuilder_.Append(System.Uri.EscapeDataString("timeformat")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(timeformat, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                    }
                    if (timezone != null)
                    {
                        urlBuilder_.Append(System.Uri.EscapeDataString("timezone")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(timezone, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                    }
                    if (past_days != null)
                    {
                        urlBuilder_.Append(System.Uri.EscapeDataString("past_days")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(past_days, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                    }
                    if (forecast_days != null)
                    {
                        urlBuilder_.Append(System.Uri.EscapeDataString("forecast_days")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(forecast_days, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                    }
                    if (forecast_hours != null)
                    {
                        urlBuilder_.Append(System.Uri.EscapeDataString("forecast_hours")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(forecast_hours, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                    }
                    if (forecast_minutely_15 != null)
                    {
                        urlBuilder_.Append(System.Uri.EscapeDataString("forecast_minutely_15")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(forecast_minutely_15, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                    }
                    if (past_hours != null)
                    {
                        urlBuilder_.Append(System.Uri.EscapeDataString("past_hours")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(past_hours, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                    }
                    if (past_minutely_15 != null)
                    {
                        urlBuilder_.Append(System.Uri.EscapeDataString("past_minutely_15")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(past_minutely_15, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                    }
                    if (start_date != null)
                    {
                        urlBuilder_.Append(System.Uri.EscapeDataString("start_date")).Append('=').Append(System.Uri.EscapeDataString(start_date.Value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                    }
                    if (end_date != null)
                    {
                        urlBuilder_.Append(System.Uri.EscapeDataString("end_date")).Append('=').Append(System.Uri.EscapeDataString(end_date.Value.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                    }
                    if (start_hour != null)
                    {
                        urlBuilder_.Append(System.Uri.EscapeDataString("start_hour")).Append('=').Append(System.Uri.EscapeDataString(start_hour.Value.ToString("s", System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                    }
                    if (end_hour != null)
                    {
                        urlBuilder_.Append(System.Uri.EscapeDataString("end_hour")).Append('=').Append(System.Uri.EscapeDataString(end_hour.Value.ToString("s", System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                    }
                    if (start_minutely_15 != null)
                    {
                        urlBuilder_.Append(System.Uri.EscapeDataString("start_minutely_15")).Append('=').Append(System.Uri.EscapeDataString(start_minutely_15.Value.ToString("s", System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                    }
                    if (end_minutely_15 != null)
                    {
                        urlBuilder_.Append(System.Uri.EscapeDataString("end_minutely_15")).Append('=').Append(System.Uri.EscapeDataString(end_minutely_15.Value.ToString("s", System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                    }
                    if (models != null)
                    {
                        urlBuilder_.Append(System.Uri.EscapeDataString("models") + "=");
                        foreach (var item_ in models)
                        {
                            urlBuilder_.Append(System.Uri.EscapeDataString(ConvertToString(item_, System.Globalization.CultureInfo.InvariantCulture))).Append(",");
                        }
                        urlBuilder_.Length--;
                        urlBuilder_.Append("&");
                    }
                    if (cell_selection != null)
                    {
                        urlBuilder_.Append(System.Uri.EscapeDataString("cell_selection")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(cell_selection, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                    }
                    if (apikey != null)
                    {
                        urlBuilder_.Append(System.Uri.EscapeDataString("apikey")).Append('=').Append(System.Uri.EscapeDataString(ConvertToString(apikey, System.Globalization.CultureInfo.InvariantCulture))).Append('&');
                    }
                    urlBuilder_.Length--;

                    PrepareRequest(client_, request_, urlBuilder_);

                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);

                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    var disposeResponse_ = true;
                    try
                    {
                        var headers_ = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>>();
                        foreach (var item_ in response_.Headers)
                            headers_[item_.Key] = item_.Value;
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }

                        ProcessResponse(client_, response_);

                        var status_ = (int)response_.StatusCode;
                        if (status_ == 200)
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<WeatherForecastResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            return new SwaggerResponse<WeatherForecastResponse>(status_, headers_, objectResponse_.Object);
                        }
                        else
                        {
                            var objectResponse_ = await ReadObjectResponseAsync<ErrorResponse>(response_, headers_, cancellationToken).ConfigureAwait(false);
                            if (objectResponse_.Object == null)
                            {
                                throw new ApiException("Response was null which was not expected.", status_, objectResponse_.Text, headers_, null);
                            }
                            throw new ApiException<ErrorResponse>("Anything other than success is an error", status_, objectResponse_.Text, headers_, objectResponse_.Object, null);
                        }
                    }
                    finally
                    {
                        if (disposeResponse_)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (disposeClient_)
                    client_.Dispose();
            }
        }

        protected struct ObjectResponseResult<T>
        {
            public ObjectResponseResult(T responseObject, string responseText)
            {
                this.Object = responseObject;
                this.Text = responseText;
            }

            public T Object { get; }

            public string Text { get; }
        }

        public bool ReadResponseAsString { get; set; }

        protected virtual async System.Threading.Tasks.Task<ObjectResponseResult<T>> ReadObjectResponseAsync<T>(System.Net.Http.HttpResponseMessage response, System.Collections.Generic.IReadOnlyDictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.Threading.CancellationToken cancellationToken)
        {
            if (response == null || response.Content == null)
            {
                return new ObjectResponseResult<T>(default(T)!, string.Empty);
            }

            if (ReadResponseAsString)
            {
                var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    var typedBody = System.Text.Json.JsonSerializer.Deserialize<T>(responseText, JsonSerializerSettings);
                    return new ObjectResponseResult<T>(typedBody!, responseText);
                }
                catch (System.Text.Json.JsonException exception)
                {
                    var message = "Could not deserialize the response body string as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, responseText, headers, exception);
                }
            }
            else
            {
                try
                {
                    using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        var typedBody = await System.Text.Json.JsonSerializer.DeserializeAsync<T>(responseStream, JsonSerializerSettings, cancellationToken).ConfigureAwait(false);
                        return new ObjectResponseResult<T>(typedBody!, string.Empty);
                    }
                }
                catch (System.Text.Json.JsonException exception)
                {
                    var message = "Could not deserialize the response body stream as " + typeof(T).FullName + ".";
                    throw new ApiException(message, (int)response.StatusCode, string.Empty, headers, exception);
                }
            }
        }

        private string ConvertToString(object? value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return "";
            }

            if (value is System.Enum)
            {
                var name = System.Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = System.Reflection.IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute)) 
                            as System.Runtime.Serialization.EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value != null ? attribute.Value : name;
                        }
                    }

                    var converted = System.Convert.ToString(System.Convert.ChangeType(value, System.Enum.GetUnderlyingType(value.GetType()), cultureInfo));
                    return converted == null ? string.Empty : converted;
                }
            }
            else if (value is bool) 
            {
                return System.Convert.ToString((bool)value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return System.Convert.ToBase64String((byte[]) value);
            }
            else if (value is string[])
            {
                return string.Join(",", (string[])value);
            }
            else if (value.GetType().IsArray)
            {
                var valueArray = (System.Array)value;
                var valueTextArray = new string[valueArray.Length];
                for (var i = 0; i < valueArray.Length; i++)
                {
                    valueTextArray[i] = ConvertToString(valueArray.GetValue(i), cultureInfo);
                }
                return string.Join(",", valueTextArray);
            }

            var result = System.Convert.ToString(value, cultureInfo);
            return result == null ? "" : result;
        }
    }

}

#pragma warning restore  108
#pragma warning restore  114
#pragma warning restore  472
#pragma warning restore  612
#pragma warning restore 1573
#pragma warning restore 1591
#pragma warning restore 8073
#pragma warning restore 3016
#pragma warning restore 8600
#pragma warning restore 8602
#pragma warning restore 8603
#pragma warning restore 8604
#pragma warning restore 8625