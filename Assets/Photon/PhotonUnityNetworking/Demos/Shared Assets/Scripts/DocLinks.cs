// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocLinks.cs" company="Exit Games GmbH">
//   Part of: Pun demos
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Photon.Pun.Demo.Shared
{
	/// <summary>
	/// Document links.
	/// </summary>
	public static class DocLinks {

		/// <summary>
		/// Document types
		/// </summary>
		public enum DocTypes {Doc,Api};

		/// <summary>
		/// The various supported languages
		/// </summary>
		public enum Products {OnPremise,Realtime,Pun,Chat,Voice,Bolt,Quantum};

		/// <summary>
		/// The various version of the documentation that exists. Current will be either V1 or V2 or possibly a beta version or branched version.
		/// </summary>
		public enum Versions {Current,V1,V2};

		/// <summary>
		/// The various supported languages
		/// </summary>
		public enum Languages {English,Japanese,Korean,Chinese};

		/// <summary>
		/// The version to generate links for
		/// </summary>
		public static Versions Version = Versions.Current;


		/// <summary>
		/// The language to generate links with
		/// </summary>
		public static Languages Language = Languages.English;

		/// <summary>
		/// The product to generate links for
		/// </summary>
		public static Products Product = Products.Pun;

		/// <summary>
		/// The API URL format.
		/// 0 is the Language
		/// 1 is the Product
		/// 2 is the Version
		/// 3 is the custom part passed to generate the link with
		/// </summary>
		public static string ApiUrlRoot = "https://doc-api.photonengine.com/{0}/{1}/{2}/{3}";

		/// <summary>
		/// The Doc URL format.
		/// 0 is the Language
		/// 1 is the Product
		/// 2 is the Version
		/// 3 is the custom part passed to generate the link with
		/// </summary>
		public static string DocUrlFormat = "https://doc.photonengine.com/{0}/{1}/{2}/{3}";


		/// <summary>
		/// LookUp dictionnary for doc versions to avoid parsing this every link request
		/// </summary>
		static Dictionary<Products,string> ProductsFolders = new Dictionary<Products, string>
		{
			{Products.Bolt, "bolt"},
			{Products.Chat, "chat"},
			{Products.OnPremise, "onpremise"},
			{Products.Pun, "pun"},
			{Products.Quantum, "quantum"},
			{Products.Realtime, "realtime"},
			{Products.Voice, "voice"}
		};

		/// <summary>
		/// LookUp dictionnary for api languages to avoid parsing this every link request
		/// </summary>
		static Dictionary<Languages,string> ApiLanguagesFolder = new Dictionary<Languages, string>
		{
			{Languages.English, "en"},
			{Languages.Japanese, "ja-jp"},
			{Languages.Korean, "ko-kr"},
			{Languages.Chinese, "zh-tw"}
		};

		/// <summary>
		/// LookUp dictionnary for doc languages to avoid parsing this every link request
		/// </summary>
		static Dictionary<Languages,string> DocLanguagesFolder = new Dictionary<Languages, string>
		{
			{Languages.English, "en-us"},
			{Languages.Japanese, "ja-jp"},
			{Languages.Korean, "ko-kr"},
			{Languages.Chinese, "en"} // fallback to english
		};

		/// <summary>
		/// LookUp dictionnary for doc versions to avoid parsing this every link request
		/// </summary>
		static Dictionary<Versions,string> VersionsFolder = new Dictionary<Versions, string>
		{
			{Versions.Current, "current"},
			{Versions.V1, "v1"},
			{Versions.V2, "v2"}
		};
			

		/// <summary>
		/// Gets a documentation link given a reference
		/// </summary>
		/// <returns>The link.</returns>
		/// <param name="type">Type.</param>
		/// <param name="reference">Reference.</param>
		public static string GetLink(DocTypes type,string reference)
		{
			if (type == DocTypes.Api) {
				return GetApiLink (reference);
			}

			if (type == DocTypes.Doc) {
				return GetDocLink (reference);
			}

			return "https://doc.photonengine.com";
		}
			
		/// <summary>
		/// Gets the API link given a reference
		/// </summary>
		/// <returns>The API link.</returns>
		/// <param name="reference">Reference.</param>
		public  static string GetApiLink(string reference)
		{
			return string.Format(ApiUrlRoot, ApiLanguagesFolder[Language],ProductsFolders[Product], VersionsFolder[Version], reference);
		}

		/// <summary>
		///  Gets the Doc link given a reference
		/// </summary>
		/// <returns>The document link.</returns>
		/// <param name="reference">Reference.</param>
		public static string GetDocLink(string reference)
		{
			return string.Format(DocUrlFormat, DocLanguagesFolder[Language],ProductsFolders[Product], VersionsFolder[Version], reference);
		}
	}
}