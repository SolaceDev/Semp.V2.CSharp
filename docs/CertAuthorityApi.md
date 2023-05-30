# Semp.V2.CSharp.Api.CertAuthorityApi

All URIs are relative to *http://www.solace.com/SEMP/v2/config*

Method | HTTP request | Description
------------- | ------------- | -------------
[**CreateCertAuthority**](CertAuthorityApi.md#createcertauthority) | **POST** /certAuthorities | Create a Certificate Authority object.
[**CreateCertAuthorityOcspTlsTrustedCommonName**](CertAuthorityApi.md#createcertauthorityocsptlstrustedcommonname) | **POST** /certAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames | Create an OCSP Responder Trusted Common Name object.
[**DeleteCertAuthority**](CertAuthorityApi.md#deletecertauthority) | **DELETE** /certAuthorities/{certAuthorityName} | Delete a Certificate Authority object.
[**DeleteCertAuthorityOcspTlsTrustedCommonName**](CertAuthorityApi.md#deletecertauthorityocsptlstrustedcommonname) | **DELETE** /certAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames/{ocspTlsTrustedCommonName} | Delete an OCSP Responder Trusted Common Name object.
[**GetCertAuthorities**](CertAuthorityApi.md#getcertauthorities) | **GET** /certAuthorities | Get a list of Certificate Authority objects.
[**GetCertAuthority**](CertAuthorityApi.md#getcertauthority) | **GET** /certAuthorities/{certAuthorityName} | Get a Certificate Authority object.
[**GetCertAuthorityOcspTlsTrustedCommonName**](CertAuthorityApi.md#getcertauthorityocsptlstrustedcommonname) | **GET** /certAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames/{ocspTlsTrustedCommonName} | Get an OCSP Responder Trusted Common Name object.
[**GetCertAuthorityOcspTlsTrustedCommonNames**](CertAuthorityApi.md#getcertauthorityocsptlstrustedcommonnames) | **GET** /certAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames | Get a list of OCSP Responder Trusted Common Name objects.
[**ReplaceCertAuthority**](CertAuthorityApi.md#replacecertauthority) | **PUT** /certAuthorities/{certAuthorityName} | Replace a Certificate Authority object.
[**UpdateCertAuthority**](CertAuthorityApi.md#updatecertauthority) | **PATCH** /certAuthorities/{certAuthorityName} | Update a Certificate Authority object.

<a name="createcertauthority"></a>
# **CreateCertAuthority**
> CertAuthorityResponse CreateCertAuthority (CertAuthority body, string opaquePassword = null, List<string> select = null)

Create a Certificate Authority object.

Create a Certificate Authority object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Clients can authenticate with the message broker over TLS by presenting a valid client certificate. The message broker authenticates the client certificate by constructing a full certificate chain (from the client certificate to intermediate CAs to a configured root CA). The intermediate CAs in this chain can be provided by the client, or configured in the message broker. The root CA must be configured on the message broker.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: certAuthorityName|x|x|||x| certContent|||||x| crlDayList|||||x| crlTimeList|||||x| crlUrl|||||x| ocspNonResponderCertEnabled|||||x| ocspOverrideUrl|||||x| ocspTimeout|||||x| revocationCheckEnabled|||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- CertAuthority|crlDayList|crlTimeList| CertAuthority|crlTimeList|crlDayList|    A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been deprecated since 2.19. Replaced by clientCertAuthorities and domainCertAuthorities.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateCertAuthorityExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertAuthorityApi();
            var body = new CertAuthority(); // CertAuthority | The Certificate Authority object's attributes.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Certificate Authority object.
                CertAuthorityResponse result = apiInstance.CreateCertAuthority(body, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertAuthorityApi.CreateCertAuthority: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**CertAuthority**](CertAuthority.md)| The Certificate Authority object&#x27;s attributes. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**CertAuthorityResponse**](CertAuthorityResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createcertauthorityocsptlstrustedcommonname"></a>
# **CreateCertAuthorityOcspTlsTrustedCommonName**
> CertAuthorityOcspTlsTrustedCommonNameResponse CreateCertAuthorityOcspTlsTrustedCommonName (CertAuthorityOcspTlsTrustedCommonName body, string certAuthorityName, string opaquePassword = null, List<string> select = null)

Create an OCSP Responder Trusted Common Name object.

Create an OCSP Responder Trusted Common Name object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  When an OCSP override URL is configured, the OCSP responder will be required to sign the OCSP responses with certificates issued to these Trusted Common Names. A maximum of 8 common names can be configured as valid response signers.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: certAuthorityName|x||x||x| ocspTlsTrustedCommonName|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been deprecated since 2.19. Replaced by clientCertAuthorities.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateCertAuthorityOcspTlsTrustedCommonNameExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertAuthorityApi();
            var body = new CertAuthorityOcspTlsTrustedCommonName(); // CertAuthorityOcspTlsTrustedCommonName | The OCSP Responder Trusted Common Name object's attributes.
            var certAuthorityName = certAuthorityName_example;  // string | The name of the Certificate Authority.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create an OCSP Responder Trusted Common Name object.
                CertAuthorityOcspTlsTrustedCommonNameResponse result = apiInstance.CreateCertAuthorityOcspTlsTrustedCommonName(body, certAuthorityName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertAuthorityApi.CreateCertAuthorityOcspTlsTrustedCommonName: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**CertAuthorityOcspTlsTrustedCommonName**](CertAuthorityOcspTlsTrustedCommonName.md)| The OCSP Responder Trusted Common Name object&#x27;s attributes. | 
 **certAuthorityName** | **string**| The name of the Certificate Authority. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**CertAuthorityOcspTlsTrustedCommonNameResponse**](CertAuthorityOcspTlsTrustedCommonNameResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletecertauthority"></a>
# **DeleteCertAuthority**
> SempMetaOnlyResponse DeleteCertAuthority (string certAuthorityName)

Delete a Certificate Authority object.

Delete a Certificate Authority object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Clients can authenticate with the message broker over TLS by presenting a valid client certificate. The message broker authenticates the client certificate by constructing a full certificate chain (from the client certificate to intermediate CAs to a configured root CA). The intermediate CAs in this chain can be provided by the client, or configured in the message broker. The root CA must be configured on the message broker.  A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been deprecated since 2.19. Replaced by clientCertAuthorities and domainCertAuthorities.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteCertAuthorityExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertAuthorityApi();
            var certAuthorityName = certAuthorityName_example;  // string | The name of the Certificate Authority.

            try
            {
                // Delete a Certificate Authority object.
                SempMetaOnlyResponse result = apiInstance.DeleteCertAuthority(certAuthorityName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertAuthorityApi.DeleteCertAuthority: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **certAuthorityName** | **string**| The name of the Certificate Authority. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletecertauthorityocsptlstrustedcommonname"></a>
# **DeleteCertAuthorityOcspTlsTrustedCommonName**
> SempMetaOnlyResponse DeleteCertAuthorityOcspTlsTrustedCommonName (string certAuthorityName, string ocspTlsTrustedCommonName)

Delete an OCSP Responder Trusted Common Name object.

Delete an OCSP Responder Trusted Common Name object. The deletion of instances of this object are synchronized to HA mates via config-sync.  When an OCSP override URL is configured, the OCSP responder will be required to sign the OCSP responses with certificates issued to these Trusted Common Names. A maximum of 8 common names can be configured as valid response signers.  A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been deprecated since 2.19. Replaced by clientCertAuthorities.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteCertAuthorityOcspTlsTrustedCommonNameExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertAuthorityApi();
            var certAuthorityName = certAuthorityName_example;  // string | The name of the Certificate Authority.
            var ocspTlsTrustedCommonName = ocspTlsTrustedCommonName_example;  // string | The expected Trusted Common Name of the OCSP responder remote certificate.

            try
            {
                // Delete an OCSP Responder Trusted Common Name object.
                SempMetaOnlyResponse result = apiInstance.DeleteCertAuthorityOcspTlsTrustedCommonName(certAuthorityName, ocspTlsTrustedCommonName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertAuthorityApi.DeleteCertAuthorityOcspTlsTrustedCommonName: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **certAuthorityName** | **string**| The name of the Certificate Authority. | 
 **ocspTlsTrustedCommonName** | **string**| The expected Trusted Common Name of the OCSP responder remote certificate. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getcertauthorities"></a>
# **GetCertAuthorities**
> CertAuthoritiesResponse GetCertAuthorities (int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Certificate Authority objects.

Get a list of Certificate Authority objects.  Clients can authenticate with the message broker over TLS by presenting a valid client certificate. The message broker authenticates the client certificate by constructing a full certificate chain (from the client certificate to intermediate CAs to a configured root CA). The intermediate CAs in this chain can be provided by the client, or configured in the message broker. The root CA must be configured on the message broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: certAuthorityName|x||x| certContent|||x| crlDayList|||x| crlTimeList|||x| crlUrl|||x| ocspNonResponderCertEnabled|||x| ocspOverrideUrl|||x| ocspTimeout|||x| revocationCheckEnabled|||x|    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been deprecated since 2.19. Replaced by clientCertAuthorities and domainCertAuthorities.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetCertAuthoritiesExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertAuthorityApi();
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Certificate Authority objects.
                CertAuthoritiesResponse result = apiInstance.GetCertAuthorities(count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertAuthorityApi.GetCertAuthorities: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**CertAuthoritiesResponse**](CertAuthoritiesResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getcertauthority"></a>
# **GetCertAuthority**
> CertAuthorityResponse GetCertAuthority (string certAuthorityName, string opaquePassword = null, List<string> select = null)

Get a Certificate Authority object.

Get a Certificate Authority object.  Clients can authenticate with the message broker over TLS by presenting a valid client certificate. The message broker authenticates the client certificate by constructing a full certificate chain (from the client certificate to intermediate CAs to a configured root CA). The intermediate CAs in this chain can be provided by the client, or configured in the message broker. The root CA must be configured on the message broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: certAuthorityName|x||x| certContent|||x| crlDayList|||x| crlTimeList|||x| crlUrl|||x| ocspNonResponderCertEnabled|||x| ocspOverrideUrl|||x| ocspTimeout|||x| revocationCheckEnabled|||x|    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been deprecated since 2.19. Replaced by clientCertAuthorities and domainCertAuthorities.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetCertAuthorityExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertAuthorityApi();
            var certAuthorityName = certAuthorityName_example;  // string | The name of the Certificate Authority.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Certificate Authority object.
                CertAuthorityResponse result = apiInstance.GetCertAuthority(certAuthorityName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertAuthorityApi.GetCertAuthority: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **certAuthorityName** | **string**| The name of the Certificate Authority. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**CertAuthorityResponse**](CertAuthorityResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getcertauthorityocsptlstrustedcommonname"></a>
# **GetCertAuthorityOcspTlsTrustedCommonName**
> CertAuthorityOcspTlsTrustedCommonNameResponse GetCertAuthorityOcspTlsTrustedCommonName (string certAuthorityName, string ocspTlsTrustedCommonName, string opaquePassword = null, List<string> select = null)

Get an OCSP Responder Trusted Common Name object.

Get an OCSP Responder Trusted Common Name object.  When an OCSP override URL is configured, the OCSP responder will be required to sign the OCSP responses with certificates issued to these Trusted Common Names. A maximum of 8 common names can be configured as valid response signers.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: certAuthorityName|x||x| ocspTlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been deprecated since 2.19. Replaced by clientCertAuthorities.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetCertAuthorityOcspTlsTrustedCommonNameExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertAuthorityApi();
            var certAuthorityName = certAuthorityName_example;  // string | The name of the Certificate Authority.
            var ocspTlsTrustedCommonName = ocspTlsTrustedCommonName_example;  // string | The expected Trusted Common Name of the OCSP responder remote certificate.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get an OCSP Responder Trusted Common Name object.
                CertAuthorityOcspTlsTrustedCommonNameResponse result = apiInstance.GetCertAuthorityOcspTlsTrustedCommonName(certAuthorityName, ocspTlsTrustedCommonName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertAuthorityApi.GetCertAuthorityOcspTlsTrustedCommonName: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **certAuthorityName** | **string**| The name of the Certificate Authority. | 
 **ocspTlsTrustedCommonName** | **string**| The expected Trusted Common Name of the OCSP responder remote certificate. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**CertAuthorityOcspTlsTrustedCommonNameResponse**](CertAuthorityOcspTlsTrustedCommonNameResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getcertauthorityocsptlstrustedcommonnames"></a>
# **GetCertAuthorityOcspTlsTrustedCommonNames**
> CertAuthorityOcspTlsTrustedCommonNamesResponse GetCertAuthorityOcspTlsTrustedCommonNames (string certAuthorityName, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of OCSP Responder Trusted Common Name objects.

Get a list of OCSP Responder Trusted Common Name objects.  When an OCSP override URL is configured, the OCSP responder will be required to sign the OCSP responses with certificates issued to these Trusted Common Names. A maximum of 8 common names can be configured as valid response signers.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: certAuthorityName|x||x| ocspTlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been deprecated since 2.19. Replaced by clientCertAuthorities.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetCertAuthorityOcspTlsTrustedCommonNamesExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertAuthorityApi();
            var certAuthorityName = certAuthorityName_example;  // string | The name of the Certificate Authority.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of OCSP Responder Trusted Common Name objects.
                CertAuthorityOcspTlsTrustedCommonNamesResponse result = apiInstance.GetCertAuthorityOcspTlsTrustedCommonNames(certAuthorityName, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertAuthorityApi.GetCertAuthorityOcspTlsTrustedCommonNames: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **certAuthorityName** | **string**| The name of the Certificate Authority. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**CertAuthorityOcspTlsTrustedCommonNamesResponse**](CertAuthorityOcspTlsTrustedCommonNamesResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacecertauthority"></a>
# **ReplaceCertAuthority**
> CertAuthorityResponse ReplaceCertAuthority (CertAuthority body, string certAuthorityName, string opaquePassword = null, List<string> select = null)

Replace a Certificate Authority object.

Replace a Certificate Authority object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Clients can authenticate with the message broker over TLS by presenting a valid client certificate. The message broker authenticates the client certificate by constructing a full certificate chain (from the client certificate to intermediate CAs to a configured root CA). The intermediate CAs in this chain can be provided by the client, or configured in the message broker. The root CA must be configured on the message broker.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- certAuthorityName|x||x||||x| certContent|||||||x| crlDayList|||||||x| crlTimeList|||||||x| crlUrl||||||x|x| ocspNonResponderCertEnabled|||||||x| ocspOverrideUrl|||||||x| ocspTimeout|||||||x| revocationCheckEnabled|||||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- CertAuthority|crlDayList|crlTimeList| CertAuthority|crlTimeList|crlDayList|    A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been deprecated since 2.19. Replaced by clientCertAuthorities and domainCertAuthorities.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceCertAuthorityExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertAuthorityApi();
            var body = new CertAuthority(); // CertAuthority | The Certificate Authority object's attributes.
            var certAuthorityName = certAuthorityName_example;  // string | The name of the Certificate Authority.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Certificate Authority object.
                CertAuthorityResponse result = apiInstance.ReplaceCertAuthority(body, certAuthorityName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertAuthorityApi.ReplaceCertAuthority: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**CertAuthority**](CertAuthority.md)| The Certificate Authority object&#x27;s attributes. | 
 **certAuthorityName** | **string**| The name of the Certificate Authority. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**CertAuthorityResponse**](CertAuthorityResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatecertauthority"></a>
# **UpdateCertAuthority**
> CertAuthorityResponse UpdateCertAuthority (CertAuthority body, string certAuthorityName, string opaquePassword = null, List<string> select = null)

Update a Certificate Authority object.

Update a Certificate Authority object. Any attribute missing from the request will be left unchanged.  Clients can authenticate with the message broker over TLS by presenting a valid client certificate. The message broker authenticates the client certificate by constructing a full certificate chain (from the client certificate to intermediate CAs to a configured root CA). The intermediate CAs in this chain can be provided by the client, or configured in the message broker. The root CA must be configured on the message broker.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- certAuthorityName|x|x||||x| certContent||||||x| crlDayList||||||x| crlTimeList||||||x| crlUrl|||||x|x| ocspNonResponderCertEnabled||||||x| ocspOverrideUrl||||||x| ocspTimeout||||||x| revocationCheckEnabled||||||x|    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- CertAuthority|crlDayList|crlTimeList| CertAuthority|crlTimeList|crlDayList|    A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been deprecated since 2.19. Replaced by clientCertAuthorities and domainCertAuthorities.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateCertAuthorityExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new CertAuthorityApi();
            var body = new CertAuthority(); // CertAuthority | The Certificate Authority object's attributes.
            var certAuthorityName = certAuthorityName_example;  // string | The name of the Certificate Authority.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Certificate Authority object.
                CertAuthorityResponse result = apiInstance.UpdateCertAuthority(body, certAuthorityName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling CertAuthorityApi.UpdateCertAuthority: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**CertAuthority**](CertAuthority.md)| The Certificate Authority object&#x27;s attributes. | 
 **certAuthorityName** | **string**| The name of the Certificate Authority. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**CertAuthorityResponse**](CertAuthorityResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
