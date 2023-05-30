# Semp.V2.CSharp.Model.CertAuthority
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**CertAuthorityName** | **string** | The name of the Certificate Authority. Deprecated since 2.19. Replaced by clientCertAuthorities and domainCertAuthorities. | [optional] 
**CertContent** | **string** | The PEM formatted content for the trusted root certificate of a Certificate Authority. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.19. certAuthorities replaced by clientCertAuthorities and domainCertAuthorities. | [optional] 
**CrlDayList** | **string** | The scheduled CRL refresh day(s), specified as \&quot;daily\&quot; or a comma-separated list of days. Days must be specified as \&quot;Sun\&quot;, \&quot;Mon\&quot;, \&quot;Tue\&quot;, \&quot;Wed\&quot;, \&quot;Thu\&quot;, \&quot;Fri\&quot;, or \&quot;Sat\&quot;, with no spaces, and in sorted order from Sunday to Saturday. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;daily\&quot;&#x60;. Deprecated since 2.19. certAuthorities replaced by clientCertAuthorities and domainCertAuthorities. | [optional] 
**CrlTimeList** | **string** | The scheduled CRL refresh time(s), specified as \&quot;hourly\&quot; or a comma-separated list of 24-hour times in the form hh:mm, or h:mm. There must be no spaces, and times must be in sorted order from 0:00 to 23:59. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;3:00\&quot;&#x60;. Deprecated since 2.19. certAuthorities replaced by clientCertAuthorities and domainCertAuthorities. | [optional] 
**CrlUrl** | **string** | The URL for the CRL source. This is a required attribute for CRL to be operational and the URL must be complete with http:// included. IPv6 addresses must be enclosed in square-brackets. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as revocationCheckEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.19. certAuthorities replaced by clientCertAuthorities and domainCertAuthorities. | [optional] 
**OcspNonResponderCertEnabled** | **bool?** | Enable or disable allowing a non-responder certificate to sign an OCSP response. Typically used with an OCSP override URL in cases where a single certificate is used to sign client certificates and OCSP responses. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.19. certAuthorities replaced by clientCertAuthorities and domainCertAuthorities. | [optional] 
**OcspOverrideUrl** | **string** | The OCSP responder URL to use for overriding the one supplied in the client certificate. The URL must be complete with http:// included. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.19. certAuthorities replaced by clientCertAuthorities and domainCertAuthorities. | [optional] 
**OcspTimeout** | **long?** | The timeout in seconds to receive a response from the OCSP responder after sending a request or making the initial connection attempt. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;5&#x60;. Deprecated since 2.19. certAuthorities replaced by clientCertAuthorities and domainCertAuthorities. | [optional] 
**RevocationCheckEnabled** | **bool?** | Enable or disable Certificate Authority revocation checking. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.19. certAuthorities replaced by clientCertAuthorities and domainCertAuthorities. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)
