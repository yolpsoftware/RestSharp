using System;

namespace RestSharp.Authenticators.OAuth
{
#if !SILVERLIGHT && !WINDOWS_PHONE && !NETFX_CORE
	[Serializable]
#endif
	public enum OAuthSignatureTreatment
	{
		Escaped,
		Unescaped
	}
}