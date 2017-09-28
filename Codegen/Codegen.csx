using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static string ToCamelCase(this string s)
{
	return s.Split('_')
		.Select(word => word.Substring(0, 1).ToUpper() + word.Substring(1))
		.Join("");
}

static string Join(this IEnumerable<string> s, string del)
{
	return string.Join(del, s);
}

static string Or(this string s, string s2)
{
	return string.IsNullOrEmpty(s) ? s2 : s;
}

struct Field
{
	public string name;
	public string comment;
	public string valueComment;
	public string type;
	public string queryField;
	public Dictionary<string, object> options;
}


var localWeatherFields = new[] {
	new Field {
		name = "location",
		comment = @"City name (ex: 'New York', 'New York,ny', 'London,united kingdom'), 
ip address (ex: '101.25.32.325'), 
US zip code, UK postcode, Canada postal code (ex: 'SW1', '90201')
or lot/lan coordinates (ex: 48.834,2.394)",
		valueComment = @"<example>New York</example>",
		type = "string",
		queryField = "q"
	},
	new Field {
		name = "num_of_days",
		comment = @"Number of days of forecast",
		valueComment = @"",
		type = "int",
		queryField = "num_of_days"
	},
	new Field {
		name = "date",
		comment = @"Specifies weather for a date",
		valueComment = @"",
		type = "DateTime",
		queryField = ""
	},
	new Field {
		name = "date_format",
		comment = @"Specifies weather for a date",
		valueComment = @"",
		type = "",
		queryField = "",
		options = new Dictionary<string, object>{
			{ "unix", "unix" },
			{ "iso8601", "iso8601" },
		}
	},
	new Field {
		name = "forecast",
		comment = @"Whether to return weather forecast output. Default: true",
		valueComment = @"",
		type = "bool",
		queryField = "fx",
		options = new Dictionary<string, object>{
		}
	},
	new Field {
		name = "conditions",
		comment = @"Whether to return current weather conditions output. Default: true",
		valueComment = @"",
		type = "bool",
		queryField = "cc",
		options = new Dictionary<string, object>{
		}
	},
	new Field {
		name = "monthly_average",
		comment = @"Whether to return monthly climate average data. Default: false",
		valueComment = @"",
		type = "bool",
		queryField = "mca",
		options = new Dictionary<string, object>{
		}
	},
	new Field {
		name = "three_hour_interval",
		comment = @"Returns 24 hourly weather information in a 3 hourly interval response. Default: false",
		valueComment = @"",
		type = "bool",
		queryField = "fx24",
		options = new Dictionary<string, object>{
		}
	},
	new Field {
		name = "include_location",
		comment = @"Whether to return the nearest weather point for which the weather data is returned for a given postcode, zipcode and lat/lon values. Default: false",
		valueComment = @"",
		type = "bool",
		queryField = "includelocation",
		options = new Dictionary<string, object>{
		}
	},
	new Field {
		name = "interval",
		comment = @"Specifies the weather forecast time interval in hours. Options are: 1 hour, 3 hourly, 6 hourly, 12 hourly (day/night) or 24 hourly (day average). Default: 3",
		valueComment = @"",
		type = "",
		queryField = "tp",
		options = new Dictionary<string, object>{
			{ "one_hour", 1},
			{ "two_hours", 2},
			{ "three_hours", 3},
			{ "six_hours", 6},
			{ "twelve_hours", 12},
			{ "twenty_four_hours", 24},
		}
	},
	new Field {
		name = "local_time",
		comment = @"Whether to return the timezone information i.e. local time and offset hour and minute for the location. Default: false",
		valueComment = @"",
		type = "bool",
		queryField = "showlocaltime",
		options = new Dictionary<string, object>{
		}
	},
	new Field {
		name = "map",
		comment = @"Returns series of GIF images for rain, snow and pressure map and surface temperature map. Default: false",
		valueComment = @"",
		type = "bool",
		queryField = "showmap",
		options = new Dictionary<string, object>{
		}
	},
	new Field {
		name = "isDayTime",
		comment = @"Adds yes for day and no for night time period. Note: This parameter only works with 3 hourly, 6 hourly or 12 hourly intervals. Default: false",
		valueComment = @"",
		type = "bool",
		queryField = "extra",
		options = new Dictionary<string, object>{
		}
	},
	new Field {
		name = "utcDateTime",
		comment = @"Time intervals are displayed in UTC format instead of locate date and time. Default: false",
		valueComment = @"",
		type = "bool",
		queryField = "extra",
		options = new Dictionary<string, object>{
		}
	},
	new Field {
		name = "localObsTime",
		comment = @"Adds the current weather observation time in UTC as well as local time of the location requested. Default: false",
		valueComment = @"",
		type = "bool",
		queryField = "extra",
		options = new Dictionary<string, object>{
		}
	},
};

static string generateEnum(Field x) {
	if (x.options == null || x.options.Count < 2) return "";
	var ret = $@"/// <summary>
		/// Available options for {x.name.ToCamelCase()}
		/// </summary>
		public enum {x.name.ToCamelCase()}Options {{
			{
				x.options.Select(kv => {
					if (kv.Value.GetType() != typeof(int)) { return kv.Key.ToUpper(); }
					else { return $"{kv.Key.ToUpper()} = {kv.Value}"; }
				})
				.Join(",\n\t\t\t")
			}
		}}
";
	return ret;
}

static string getType(Field x) {
	var t = x.options == null || x.options.Keys.Count < 2 ? x.type : x.name.ToCamelCase() + "Options";
	return t == "string" ? t : t + "?";
}

var fields = String.Join("\n\n", localWeatherFields.Select(x => $@"
		{generateEnum(x)}
		/// <summary>
		/// {x.name.ToCamelCase()}: {x.comment.Split('\n').Join("\n\t\t///<para /> ")}
		/// </summary>
		public {getType(x)} {x.name.ToCamelCase()} {{ get; set; }}

		/// <summary>
		/// Sets {x.name.ToCamelCase()} and returns modified query
		/// </summary>
		/// <param name=""{x.name}"">{x.comment.Split('\n').Join("\n\t\t///<para /> ")}</param>
		/// <returns>this</returns>
		public LocalWeatherQuery With{x.name.ToCamelCase()} ({getType(x)} {x.name}) {{ 
			this.{x.name.ToCamelCase()} = {x.name};
			return this;
		}}
"));

static string getQueryStringVal(Field x) {
	if (x.type == "bool")
	{
		return $@"{{({x.name.ToCamelCase()}.Value ? ""yes"" : ""no"")}}";
	}
	if (x.type == "DateTime")
	{
		return $@"{{{x.name.ToCamelCase()}.Value.ToString(""yyyy-MM-dd"")}}";
	}
	if (x.options != null && x.options.Count > 1) {
		if (x.options.Values.First().GetType() == typeof(int))
		{
			return $@"{{(int){x.name.ToCamelCase()}.Value}}";
		}
		else {
			return $@"{{{x.name.ToCamelCase()}.ToString().ToLower()}}";
		}
	}
	return $@"{{{x.name.ToCamelCase()}}}";
}


Output["../WorldWeatherOnlineApi/LocalWeatherQuery.gen.cs"].WriteLine($@"
using System;

namespace WorldWeatherOnline
{{
	/// <summary>
	/// Class used to construct query to local weather Api 
	/// </summary>
	/// <remarks>
	/// Full documentation for REST service can be found at https://developer.worldweatheronline.com/api/docs/local-city-town-weather-api.aspx
	/// </remarks>
	public partial class LocalWeatherQuery
    {{
		{fields}

		private string _buildQueryString(){{
			var s = """";
			{
			localWeatherFields.Where(x=>x.queryField != "extra")
			.Select(x=> $@"if({x.name.ToCamelCase()} != null){{
				s+= $""&{x.queryField.Or(x.name)}={getQueryStringVal(x)}"";
			}}")
				.Join("\n\t\t\t")
		}
			return ""weather.ashx?"" + s.Substring(1);
		}}
    }}
}}
");

Output["../WorldWeatherOnline/LocalWeatherQuery.gen.cs"].WriteLine($@"
using System;

namespace WorldWeatherOnline
{{
	/// <summary>
	/// Class used to construct query to local weather Api 
	/// </summary>
	/// <remarks>
	/// Full documentation for REST service can be found at https://developer.worldweatheronline.com/api/docs/local-city-town-weather-api.aspx
	/// </remarks>
	public partial class LocalWeatherQuery
    {{
		{fields}

		private string _buildQueryString(){{
			var s = """";
			{
			localWeatherFields.Where(x => x.queryField != "extra")
			.Select(x => $@"if({x.name.ToCamelCase()} != null){{
				s+= $""&{x.queryField.Or(x.name)}={getQueryStringVal(x)}"";
			}}")
				.Join("\n\t\t\t")
		}
			return ""weather.ashx?"" + s.Substring(1);
		}}
    }}
}}
");


var pastWeatherFields = new[] {
	new Field {
		name = "location",
		comment = @"City name (ex: 'New York', 'New York,ny', 'London,united kingdom'), 
ip address (ex: '101.25.32.325'), 
US zip code, UK postcode, Canada postal code (ex: 'SW1', '90201')
or lot/lan coordinates (ex: 48.834,2.394)",
		valueComment = @"<example>New York</example>",
		type = "string",
		queryField = "q"
	},
	new Field {
		name = "date",
		comment = @"Specifies weather for a date",
		valueComment = @"",
		type = "DateTime",
		queryField = ""
	},
	new Field {
		name = "enddate",
		comment = @"If you wish to retrieve weather between two dates, use this parameter to specify the ending date. 
Important: the enddate parameter must have the same month and year as the date parameter.",
		valueComment = @"",
		type = "DateTime",
		queryField = ""
	},
	new Field {
		name = "include_location",
		comment = @"Whether to return the nearest weather point for which the weather data is returned for a given postcode, zipcode and lat/lon values. Default: false",
		valueComment = @"",
		type = "bool",
		queryField = "includelocation",
		options = new Dictionary<string, object>{
		}
	},
	new Field {
		name = "interval",
		comment = @"Specifies the weather forecast time interval in hours. Options are: 1 hour, 3 hourly, 6 hourly, 12 hourly (day/night) or 24 hourly (day average). Default: 3",
		valueComment = @"",
		type = "",
		queryField = "tp",
		options = new Dictionary<string, object>{
			{ "one_hour", 1},
			{ "two_hours", 2},
			{ "three_hours", 3},
			{ "six_hours", 6},
			{ "twelve_hours", 12},
			{ "twenty_four_hours", 24},
		}
	},
	new Field {
		name = "isDayTime",
		comment = @"Adds yes for day and no for night time period. Note: This parameter only works with 3 hourly, 6 hourly or 12 hourly intervals. Default: false",
		valueComment = @"",
		type = "bool",
		queryField = "extra",
		options = new Dictionary<string, object>{
		}
	},
	new Field {
		name = "utcDateTime",
		comment = @"Time intervals are displayed in UTC format instead of locate date and time. Default: false",
		valueComment = @"",
		type = "bool",
		queryField = "extra",
		options = new Dictionary<string, object>{
		}
	},
};

var past_fields = String.Join("\n\n", pastWeatherFields.Select(x => $@"
		{generateEnum(x)}
		/// <summary>
		/// {x.name.ToCamelCase()}: {x.comment.Split('\n').Join("\n\t\t///<para /> ")}
		/// </summary>
		public {getType(x)} {x.name.ToCamelCase()} {{ get; set; }}

		/// <summary>
		/// Sets {x.name.ToCamelCase()} and returns modified query
		/// </summary>
		/// <param name=""{x.name}"">{x.comment.Split('\n').Join("\n\t\t///<para /> ")}</param>
		/// <returns>this</returns>
		public PastWeatherQuery With{x.name.ToCamelCase()} ({getType(x)} {x.name}) {{ 
			this.{x.name.ToCamelCase()} = {x.name};
			return this;
		}}
"));

Output["../WorldWeatherOnlineApi/PastWeatherQuery.gen.cs"].WriteLine($@"
using System;

namespace WorldWeatherOnline
{{
	/// <summary>
	/// Class used to construct query to local weather Api 
	/// </summary>
	/// <remarks>
	/// Full documentation for REST service can be found at https://developer.worldweatheronline.com/api/docs/historical-weather-api.aspx
	/// </remarks>
	public partial class PastWeatherQuery
    {{
		{past_fields}

		private string _buildQueryString(){{
			var s = """";
			{
			pastWeatherFields.Where(x => x.queryField != "extra")
			.Select(x => $@"if({x.name.ToCamelCase()} != null){{
				s+= $""&{x.queryField.Or(x.name)}={getQueryStringVal(x)}"";
			}}")
				.Join("\n\t\t\t")
		}
			return ""past-weather.ashx?"" + s.Substring(1);
		}}
    }}
}}
");

Output["../WorldWeatherOnline/PastWeatherQuery.gen.cs"].WriteLine($@"
using System;

namespace WorldWeatherOnline
{{
	/// <summary>
	/// Class used to construct query to historical weather Api 
	/// </summary>
	/// <remarks>
	/// Full documentation for REST service can be found at https://developer.worldweatheronline.com/api/docs/historical-weather-api.aspx
	/// </remarks>
	public partial class PastWeatherQuery
    {{
		{past_fields}

		private string _buildQueryString(){{
			var s = """";
			{
			pastWeatherFields.Where(x => x.queryField != "extra")
			.Select(x => $@"if({x.name.ToCamelCase()} != null){{
				s+= $""&{x.queryField.Or(x.name)}={getQueryStringVal(x)}"";
			}}")
				.Join("\n\t\t\t")
		}
			return ""past-weather.ashx?"" + s.Substring(1);
		}}
    }}
}}
");



var marineWeatherFields = new[] {
	new Field {
		name = "location",
		comment = @"City name (ex: 'New York', 'New York,ny', 'London,united kingdom'), 
ip address (ex: '101.25.32.325'), 
US zip code, UK postcode, Canada postal code (ex: 'SW1', '90201')
or lot/lan coordinates (ex: 48.834,2.394)",
		valueComment = @"<example>New York</example>",
		type = "string",
		queryField = "q"
	},
	new Field {
		name = "forecast",
		comment = @"Whether to return weather forecast output. Default: true",
		valueComment = @"",
		type = "bool",
		queryField = "fx",
		options = new Dictionary<string, object>{
		}
	},
	new Field {
		name = "tide",
		comment = @"To return tide data information if available. Default: false",
		valueComment = @"",
		type = "bool",
		queryField = "tide",
		options = new Dictionary<string, object>{
		}
	},
	new Field {
		name = "interval",
		comment = @"Specifies the weather forecast time interval in hours. Options are: 1 hour, 3 hourly, 6 hourly, 12 hourly (day/night) or 24 hourly (day average). Default: 3",
		valueComment = @"",
		type = "",
		queryField = "tp",
		options = new Dictionary<string, object>{
			{ "one_hour", 1},
			{ "two_hours", 2},
			{ "three_hours", 3},
			{ "six_hours", 6},
			{ "twelve_hours", 12},
			{ "twenty_four_hours", 24},
		}
	},
	new Field {
		name = "include_location",
		comment = @"Whether to return the nearest weather point for which the weather data is returned for a given postcode, zipcode and lat/lon values. Default: false",
		valueComment = @"",
		type = "bool",
		queryField = "includelocation",
		options = new Dictionary<string, object>{
		}
	},
};

var marine_fields = String.Join("\n\n", marineWeatherFields.Select(x => $@"
		{generateEnum(x)}
		/// <summary>
		/// {x.name.ToCamelCase()}: {x.comment.Split('\n').Join("\n\t\t///<para /> ")}
		/// </summary>
		public {getType(x)} {x.name.ToCamelCase()} {{ get; set; }}

		/// <summary>
		/// Sets {x.name.ToCamelCase()} and returns modified query
		/// </summary>
		/// <param name=""{x.name}"">{x.comment.Split('\n').Join("\n\t\t///<para /> ")}</param>
		/// <returns>this</returns>
		public MarineWeatherQuery With{x.name.ToCamelCase()} ({getType(x)} {x.name}) {{ 
			this.{x.name.ToCamelCase()} = {x.name};
			return this;
		}}
"));

Output["../WorldWeatherOnlineApi/MarineWeatherQuery.gen.cs"].WriteLine($@"
using System;

namespace WorldWeatherOnline
{{
	/// <summary>
	/// Class used to construct query to marine weather Api 
	/// </summary>
	/// <remarks>
	/// Full documentation for REST service can be found at https://developer.worldweatheronline.com/api/docs/marine-weather-api.aspx
	/// </remarks>
	public partial class MarineWeatherQuery
    {{
		{marine_fields}

		private string _buildQueryString(){{
			var s = """";
			{
			marineWeatherFields.Where(x => x.queryField != "extra")
			.Select(x => $@"if({x.name.ToCamelCase()} != null){{
				s+= $""&{x.queryField.Or(x.name)}={getQueryStringVal(x)}"";
			}}")
				.Join("\n\t\t\t")
		}
			return ""marine.ashx?"" + s.Substring(1);
		}}
    }}
}}
");

Output["../WorldWeatherOnline/MarineWeatherQuery.gen.cs"].WriteLine($@"
using System;

namespace WorldWeatherOnline
{{
	/// <summary>
	/// Class used to construct query to marine weather Api 
	/// </summary>
	/// <remarks>
	/// Full documentation for REST service can be found at https://developer.worldweatheronline.com/api/docs/marine-weather-api.aspx
	/// </remarks>
	public partial class MarineWeatherQuery
    {{
		{marine_fields}

		private string _buildQueryString(){{
			var s = """";
			{
			marineWeatherFields.Where(x => x.queryField != "extra")
			.Select(x => $@"if({x.name.ToCamelCase()} != null){{
				s+= $""&{x.queryField.Or(x.name)}={getQueryStringVal(x)}"";
			}}")
				.Join("\n\t\t\t")
		}
			return ""marine.ashx?"" + s.Substring(1);
		}}
    }}
}}
");




var past_marineWeatherFields = new[] {
	new Field {
		name = "location",
		comment = @"City name (ex: 'New York', 'New York,ny', 'London,united kingdom'), 
ip address (ex: '101.25.32.325'), 
US zip code, UK postcode, Canada postal code (ex: 'SW1', '90201')
or lot/lan coordinates (ex: 48.834,2.394)",
		valueComment = @"<example>New York</example>",
		type = "string",
		queryField = "q"
	},
	new Field {
		name = "tide",
		comment = @"To return tide data information if available. Default: false",
		valueComment = @"",
		type = "bool",
		queryField = "tide",
		options = new Dictionary<string, object>{
		}
	},
	new Field {
		name = "date",
		comment = @"Specifies weather for a date",
		valueComment = @"",
		type = "DateTime",
		queryField = ""
	},
	new Field {
		name = "enddate",
		comment = @"Specifies end date",
		valueComment = @"",
		type = "DateTime",
		queryField = ""
	},
	new Field {
		name = "include_location",
		comment = @"Whether to return the nearest weather point for which the weather data is returned for a given postcode, zipcode and lat/lon values. Default: false",
		valueComment = @"",
		type = "bool",
		queryField = "includelocation",
		options = new Dictionary<string, object>{
		}
	},
};

var past_marine_fields = String.Join("\n\n", past_marineWeatherFields.Select(x => $@"
		{generateEnum(x)}
		/// <summary>
		/// {x.name.ToCamelCase()}: {x.comment.Split('\n').Join("\n\t\t///<para /> ")}
		/// </summary>
		public {getType(x)} {x.name.ToCamelCase()} {{ get; set; }}

		/// <summary>
		/// Sets {x.name.ToCamelCase()} and returns modified query
		/// </summary>
		/// <param name=""{x.name}"">{x.comment.Split('\n').Join("\n\t\t///<para /> ")}</param>
		/// <returns>this</returns>
		public PastMarineWeatherQuery With{x.name.ToCamelCase()} ({getType(x)} {x.name}) {{ 
			this.{x.name.ToCamelCase()} = {x.name};
			return this;
		}}
"));

foreach (var tpl in new[] { "../WorldWeatherOnlineApi/PastMarineWeatherQuery.gen.cs", "../WorldWeatherOnline/PastMarineWeatherQuery.gen.cs" }) {
Output[tpl].WriteLine($@"
using System;

namespace WorldWeatherOnline
{{
	/// <summary>
	/// Class used to construct query to historical marine weather Api 
	/// </summary>
	/// <remarks>
	/// Full documentation for REST service can be found at https://developer.worldweatheronline.com/api/docs/historical-marine-weather-api.aspx
	/// </remarks>
	public partial class PastMarineWeatherQuery
    {{
		{past_marine_fields}

		private string _buildQueryString(){{
			var s = """";
			{
				past_marineWeatherFields.Where(x => x.queryField != "extra")
				.Select(x => $@"if({x.name.ToCamelCase()} != null){{
				s+= $""&{x.queryField.Or(x.name)}={getQueryStringVal(x)}"";
			}}")
					.Join("\n\t\t\t")
			}
			return ""past-marine.ashx?"" + s.Substring(1);
		}}
    }}
}}
");
}


var skiWeatherFields = new[] {
	new Field {
		name = "location",
		comment = @"Finds the nearest ski and mountain location for the provided search criteria.
City name (ex: 'New York', 'New York,ny', 'London,united kingdom'), 
ip address (ex: '101.25.32.325'), 
US zip code, UK postcode, Canada postal code (ex: 'SW1', '90201')
or lot/lan coordinates (ex: 48.834,2.394)",
		valueComment = @"<example>New York</example>",
		type = "string",
		queryField = "q"
	},
	new Field {
		name = "num_of_days",
		comment = @"Number of days of forecast",
		valueComment = @"",
		type = "int",
		queryField = "num_of_days"
	},
	new Field {
		name = "date",
		comment = @"Specifies weather for a date",
		valueComment = @"",
		type = "DateTime",
		queryField = ""
	},

	new Field {
		name = "include_location",
		comment = @"Whether to return the nearest weather point for which the weather data is returned for a given postcode, zipcode and lat/lon values. Default: false",
		valueComment = @"",
		type = "bool",
		queryField = "includelocation",
		options = new Dictionary<string, object>{
		}
	},

	new Field {
		name = "isDayTime",
		comment = @"Adds yes for day and no for night time period. Note: This parameter only works with 3 hourly, 6 hourly or 12 hourly intervals. Default: false",
		valueComment = @"",
		type = "bool",
		queryField = "extra",
		options = new Dictionary<string, object>{
		}
	},
};

var ski_fields = String.Join("\n\n", skiWeatherFields.Select(x => $@"
		{generateEnum(x)}
		/// <summary>
		/// {x.name.ToCamelCase()}: {x.comment.Split('\n').Join("\n\t\t///<para /> ")}
		/// </summary>
		public {getType(x)} {x.name.ToCamelCase()} {{ get; set; }}

		/// <summary>
		/// Sets {x.name.ToCamelCase()} and returns modified query
		/// </summary>
		/// <param name=""{x.name}"">{x.comment.Split('\n').Join("\n\t\t///<para /> ")}</param>
		/// <returns>this</returns>
		public SkiWeatherQuery With{x.name.ToCamelCase()} ({getType(x)} {x.name}) {{ 
			this.{x.name.ToCamelCase()} = {x.name};
			return this;
		}}
"));

foreach (var tpl in new[] { "../WorldWeatherOnlineApi/SkiWeatherQuery.gen.cs", "../WorldWeatherOnline/SkiWeatherQuery.gen.cs" })
{
	Output[tpl].WriteLine($@"
using System;

namespace WorldWeatherOnline
{{
	/// <summary>
	/// Class used to construct query to ski weather Api 
	/// </summary>
	/// <remarks>
	/// Full documentation for REST service can be found at https://developer.worldweatheronline.com/api/docs/ski-weather-api.aspx
	/// </remarks>
	public partial class SkiWeatherQuery
    {{
		{ski_fields}

		private string _buildQueryString(){{
			var s = """";
			{
					skiWeatherFields.Where(x => x.queryField != "extra")
					.Select(x => $@"if({x.name.ToCamelCase()} != null){{
				s+= $""&{x.queryField.Or(x.name)}={getQueryStringVal(x)}"";
			}}")
						.Join("\n\t\t\t")
				}
			return ""ski.ashx?"" + s.Substring(1);
		}}
    }}
}}
");
}


var searchWeatherFields = new[] {
	new Field {
		name = "location",
		comment = @"Finds the nearest ski and mountain location for the provided search criteria.
City name (ex: 'New York', 'New York,ny', 'London,united kingdom'), 
ip address (ex: '101.25.32.325'), 
US zip code, UK postcode, Canada postal code (ex: 'SW1', '90201')
or lot/lan coordinates (ex: 48.834,2.394)",
		valueComment = @"<example>New York</example>",
		type = "string",
		queryField = "q"
	},
	new Field {
		name = "num_of_results",
		comment = @"The number of results to return. Free API: Default 3, maximum 3. Premium API: Default 10, maximum 50",
		valueComment = @"",
		type = "int",
		queryField = "num_of_results"
	},
	new Field {
		name = "timezone",
		comment = @"Whether to return offset hours from GMT for each location. Default: false",
		valueComment = @"",
		type = "bool",
		queryField = "timezone"
	},
	new Field {
		name = "popular",
		comment = @"Whether to only search for popular locations, such as major cities. Default: false",
		valueComment = @"",
		type = "bool",
		queryField = "popular"
	},
	new Field {
		name = "wct",
		comment = @"Returns nearest locations for the type of category provided. E.g:- wct=Ski or wct=Cricket",
		valueComment = @"",
		type = "",
		queryField = "popular",
		options = new Dictionary<string, object>{
			{ "Ski", "Ski"},
			{ "Cricket", "Cricket"},
			{ "Football", "Football"},
			{ "Golf", "Golf"},
			{ "Fishing", "Fishing"},
		}
	},

};

var search_fields = String.Join("\n\n", searchWeatherFields.Select(x => $@"
		{generateEnum(x)}
		/// <summary>
		/// {x.name.ToCamelCase()}: {x.comment.Split('\n').Join("\n\t\t///<para /> ")}
		/// </summary>
		public {getType(x)} {x.name.ToCamelCase()} {{ get; set; }}

		/// <summary>
		/// Sets {x.name.ToCamelCase()} and returns modified query
		/// </summary>
		/// <param name=""{x.name}"">{x.comment.Split('\n').Join("\n\t\t///<para /> ")}</param>
		/// <returns>this</returns>
		public SearchWeatherQuery With{x.name.ToCamelCase()} ({getType(x)} {x.name}) {{ 
			this.{x.name.ToCamelCase()} = {x.name};
			return this;
		}}
"));

foreach (var tpl in new[] { "../WorldWeatherOnlineApi/SearchWeatherQuery.gen.cs", "../WorldWeatherOnline/SearchWeatherQuery.gen.cs" })
{
	Output[tpl].WriteLine($@"
using System;

namespace WorldWeatherOnline
{{
	/// <summary>
	/// The Location search REST API returns information about the location, including area name, country, latitude/longitude, population, and a URL for the weather information. Note that the Free API returns 30 results and the Premium API returns 100 results.
	/// </summary>
	/// <remarks>
	/// Full documentation for REST service can be found at https://developer.worldweatheronline.com/api/docs/search-api.aspx
	/// </remarks>
	public partial class SearchWeatherQuery
    {{
		{search_fields}

		private string _buildQueryString(){{
			var s = """";
			{
					searchWeatherFields.Where(x => x.queryField != "extra")
					.Select(x => $@"if({x.name.ToCamelCase()} != null){{
				s+= $""&{x.queryField.Or(x.name)}={getQueryStringVal(x)}"";
			}}")
						.Join("\n\t\t\t")
				}
			return ""search.ashx?"" + s.Substring(1);
		}}
    }}
}}
");
}



var tzWeatherFields = new[] {
	new Field {
		name = "location",
		comment = @"Finds the nearest ski and mountain location for the provided search criteria.
City name (ex: 'New York', 'New York,ny', 'London,united kingdom'), 
ip address (ex: '101.25.32.325'), 
US zip code, UK postcode, Canada postal code (ex: 'SW1', '90201')
or lot/lan coordinates (ex: 48.834,2.394)",
		valueComment = @"<example>New York</example>",
		type = "string",
		queryField = "q"
	},

	new Field {
		name = "include_location",
		comment = @"Whether to return the nearest weather point for which the weather data is returned for a given postcode, zipcode and lat/lon values. Default: false",
		valueComment = @"",
		type = "bool",
		queryField = "includelocation",
		options = new Dictionary<string, object>{
		}
	},
};

var tz_fields = String.Join("\n\n", tzWeatherFields.Select(x => $@"
		{generateEnum(x)}
		/// <summary>
		/// {x.name.ToCamelCase()}: {x.comment.Split('\n').Join("\n\t\t///<para /> ")}
		/// </summary>
		public {getType(x)} {x.name.ToCamelCase()} {{ get; set; }}

		/// <summary>
		/// Sets {x.name.ToCamelCase()} and returns modified query
		/// </summary>
		/// <param name=""{x.name}"">{x.comment.Split('\n').Join("\n\t\t///<para /> ")}</param>
		/// <returns>this</returns>
		public TzWeatherQuery With{x.name.ToCamelCase()} ({getType(x)} {x.name}) {{ 
			this.{x.name.ToCamelCase()} = {x.name};
			return this;
		}}
"));

foreach (var tpl in new[] { "../WorldWeatherOnlineApi/TzWeatherQuery.gen.cs", "../WorldWeatherOnline/TzWeatherQuery.gen.cs" })
{
	Output[tpl].WriteLine($@"
using System;

namespace WorldWeatherOnline
{{
	/// <summary>
	/// Query to timezone service endpoint
	/// </summary>
	/// <remarks>
	/// Full documentation for REST service can be found at https://developer.worldweatheronline.com/api/docs/time-zone-api.aspx
	/// </remarks>
	public partial class TzWeatherQuery
    {{
		{tz_fields}

		private string _buildQueryString(){{
			var s = """";
			{
					tzWeatherFields.Where(x => x.queryField != "extra")
					.Select(x => $@"if({x.name.ToCamelCase()} != null){{
				s+= $""&{x.queryField.Or(x.name)}={getQueryStringVal(x)}"";
			}}")
						.Join("\n\t\t\t")
				}
			return ""tz.ashx?"" + s.Substring(1);
		}}
    }}
}}
");
}