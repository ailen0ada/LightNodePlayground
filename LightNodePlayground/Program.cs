using System;
using Owin;
using Microsoft.Owin.Hosting;
using LightNode.Server;
using LightNode.Formatter;
using LightNode.Swagger;
using System.Threading.Tasks;

namespace LightNodePlayground
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (WebApp.Start<Startup> ("http://*:5004/")) {
				Console.ReadLine ();
			}
		}
	}

	/// <summary>
	/// Startup.
	/// </summary>
	public class Startup
	{
		/// <summary>
		/// Configuration the specified app.
		/// </summary>
		/// <param name="app">App.</param>
		public void Configuration (IAppBuilder app)
		{
			app.UseErrorPage (new Microsoft.Owin.Diagnostics.ErrorPageOptions{ ShowExceptionDetails = true });

			app.Map ("/api", builder =>
				builder.UseLightNode (new LightNodeOptions (AcceptVerbs.Get | AcceptVerbs.Post, new JilContentFormatter (), new GZipJilContentFormatter ()) {
				ParameterEnumAllowsFieldNameParse = true,
				// If you want to use enums human readable display on Swagger, set to true
				ErrorHandlingPolicy = ErrorHandlingPolicy.ReturnInternalServerErrorIncludeErrorDetails,
				OperationMissingHandlingPolicy = OperationMissingHandlingPolicy.ReturnErrorStatusCodeIncludeErrorDetails
			}));

			app.Map ("/swagger", builder => {
				// If you want to additional info for Swagger, load xmlDoc file.
				// LightNode.Swagger loads methods's summary, remarks, param for info.     
				var xmlName = "LightNodePlayground.xml";
				var xmlPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, xmlName); // or HttpContext.Current.Server.MapPath("~/bin/" + xmlName);

				//System.Diagnostics.Debug.WriteLine(System.IO.File.Exists(xmlPath));
				builder.UseLightNodeSwagger (new SwaggerOptions ("LightNodePlayground", "/api") { // baseApi is LightNode's root
					XmlDocumentPath = xmlPath,
					IsEmitEnumAsString = true
				});
			});
		}
	}

	/// <summary>
	/// Sample contract.
	/// </summary>
	public class Sample : LightNodeContract
	{
		/// <summary>
		/// Say hello to the specified name.
		/// </summary>
		/// <param name="name">Name.</param>
		public Response Hello (string name)
		{
			return new Response { Greetings = string.Format ("Hello, {0}!", name) };
		}
	}

	/// <summary>
	/// Response.
	/// </summary>
	public class Response
	{
		/// <summary>
		/// Gets or sets the greetings.
		/// </summary>
		/// <value>The greetings.</value>
		public string Greetings { get; set; }

		/// <summary>
		/// Gets the environment.
		/// </summary>
		/// <value>The environment.</value>
		public string Environment{ get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="LightNodePlayground.Response"/> class.
		/// </summary>
		public Response ()
		{
			Environment = System.Environment.Version.ToString ();
		}
	}
}
