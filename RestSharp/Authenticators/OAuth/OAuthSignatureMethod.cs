using System;

namespace RestSharp.Authenticators.OAuth
{
#if !SILVERLIGHT && !WINDOWS_PHONE && !NETFX_CORE
	[Serializable]
#endif
	public enum OAuthSignatureMethod
	{
		HmacSha1,
		PlainText,
		RsaSha1
	}
}