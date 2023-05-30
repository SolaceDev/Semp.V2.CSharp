# Semp.V2.CSharp.Api.ClientCertAuthorityApi

All URIs are relative to *http://www.solace.com/SEMP/v2/config*

Method | HTTP request | Description
------------- | ------------- | -------------
[**CreateClientCertAuthority**](ClientCertAuthorityApi.md#createclientcertauthority) | **POST** /clientCertAuthorities | Create a Client Certificate Authority object.
[**CreateClientCertAuthorityOcspTlsTrustedCommonName**](ClientCertAuthorityApi.md#createclientcertauthorityocsptlstrustedcommonname) | **POST** /clientCertAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames | Create an OCSP Responder Trusted Common Name object.
[**DeleteClientCertAuthority**](ClientCertAuthorityApi.md#deleteclientcertauthority) | **DELETE** /clientCertAuthorities/{certAuthorityName} | Delete a Client Certificate Authority object.
[**DeleteClientCertAuthorityOcspTlsTrustedCommonName**](ClientCertAuthorityApi.md#deleteclientcertauthorityocsptlstrustedcommonname) | **DELETE** /clientCertAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames/{ocspTlsTrustedCommonName} | Delete an OCSP Responder Trusted Common Name object.
[**GetClientCertAuthorities**](ClientCertAuthorityApi.md#getclientcertauthorities) | **GET** /clientCertAuthorities | Get a list of Client Certificate Authority objects.
[**GetClientCertAuthority**](ClientCertAuthorityApi.md#getclientcertauthority) | **GET** /clientCertAuthorities/{certAuthorityName} | Get a Client Certificate Authority object.
[**GetClientCertAuthorityOcspTlsTrustedCommonName**](ClientCertAuthorityApi.md#getclientcertauthorityocsptlstrustedcommonname) | **GET** /clientCertAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames/{ocspTlsTrustedCommonName} | Get an OCSP Responder Trusted Common Name object.
[**GetClientCertAuthorityOcspTlsTrustedCommonNames**](ClientCertAuthorityApi.md#getclientcertauthorityocsptlstrustedcommonnames) | **GET** /clientCertAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames | Get a list of OCSP Responder Trusted Common Name objects.
[**ReplaceClientCertAuthority**](ClientCertAuthorityApi.md#replaceclientcertauthority) | **PUT** /clientCertAuthorities/{certAuthorityName} | Replace a Client Certificate Authority object.
[**UpdateClientCertAuthority**](ClientCertAuthorityApi.md#updateclientcertauthority) | **PATCH** /clientCertAuthorities/{certAuthorityName} | Update a Client Certificate Authority object.

<a name="createclientcertauthority"></a>
# **CreateClientCertAuthority**
> ClientCertAuthorityResponse CreateClientCertAuthority (ClientCertAuthority body, string opaquePassword = null, List<string> select = null)

Create a Client Certificate Authority object.

Create a Client Certificate Authority object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Clients can authenticate with the message broker over TLS by presenting a valid client certificate. The message broker authenticates the client certificate by constructing a full certificate chain (from the client certificate to intermediate CAs to a configured root CA). The intermediate CAs in this chain can be provided by the client, or configured in the message broker. The root CA must be configured on the message broker.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: certAuthorityName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- ClientCertAuthority|crlDayList|crlTimeList| ClientCertAuthority|crlTimeList|crlDayList|    A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been available since 2.19.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateClientCertAuthorityExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new ClientCertAuthorityApi();
            var body = new ClientCertAuthority(); // ClientCertAuthority | The Client Certificate Authority object's attributes.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Client Certificate Authority object.
                ClientCertAuthorityResponse result = apiInstance.CreateClientCertAuthority(body, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ClientCertAuthorityApi.CreateClientCertAuthority: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**ClientCertAuthority**](ClientCertAuthority.md)| The Client Certificate Authority object&#x27;s attributes. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**ClientCertAuthorityResponse**](ClientCertAuthorityResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createclientcertauthorityocsptlstrustedcommonname"></a>
# **CreateClientCertAuthorityOcspTlsTrustedCommonName**
> ClientCertAuthorityOcspTlsTrustedCommonNameResponse CreateClientCertAuthorityOcspTlsTrustedCommonName (ClientCertAuthorityOcspTlsTrustedCommonName body, string certAuthorityName, string opaquePassword = null, List<string> select = null)

Create an OCSP Responder Trusted Common Name object.

Create an OCSP Responder Trusted Common Name object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  When an OCSP override URL is configured, the OCSP responder will be required to sign the OCSP responses with certificates issued to these Trusted Common Names. A maximum of 8 common names can be configured as valid response signers.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: certAuthorityName|x||x||| ocspTlsTrustedCommonName|x|x||||    A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been available since 2.19.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateClientCertAuthorityOcspTlsTrustedCommonNameExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new ClientCertAuthorityApi();
            var body = new ClientCertAuthorityOcspTlsTrustedCommonName(); // ClientCertAuthorityOcspTlsTrustedCommonName | The OCSP Responder Trusted Common Name object's attributes.
            var certAuthorityName = certAuthorityName_example;  // string | The name of the Certificate Authority.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create an OCSP Responder Trusted Common Name object.
                ClientCertAuthorityOcspTlsTrustedCommonNameResponse result = apiInstance.CreateClientCertAuthorityOcspTlsTrustedCommonName(body, certAuthorityName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ClientCertAuthorityApi.CreateClientCertAuthorityOcspTlsTrustedCommonName: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**ClientCertAuthorityOcspTlsTrustedCommonName**](ClientCertAuthorityOcspTlsTrustedCommonName.md)| The OCSP Responder Trusted Common Name object&#x27;s attributes. | 
 **certAuthorityName** | **string**| The name of the Certificate Authority. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**ClientCertAuthorityOcspTlsTrustedCommonNameResponse**](ClientCertAuthorityOcspTlsTrustedCommonNameResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deleteclientcertauthority"></a>
# **DeleteClientCertAuthority**
> SempMetaOnlyResponse DeleteClientCertAuthority (string certAuthorityName)

Delete a Client Certificate Authority object.

Delete a Client Certificate Authority object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Clients can authenticate with the message broker over TLS by presenting a valid client certificate. The message broker authenticates the client certificate by constructing a full certificate chain (from the client certificate to intermediate CAs to a configured root CA). The intermediate CAs in this chain can be provided by the client, or configured in the message broker. The root CA must be configured on the message broker.  A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been available since 2.19.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteClientCertAuthorityExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new ClientCertAuthorityApi();
            var certAuthorityName = certAuthorityName_example;  // string | The name of the Certificate Authority.

            try
            {
                // Delete a Client Certificate Authority object.
                SempMetaOnlyResponse result = apiInstance.DeleteClientCertAuthority(certAuthorityName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ClientCertAuthorityApi.DeleteClientCertAuthority: " + e.Message );
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
<a name="deleteclientcertauthorityocsptlstrustedcommonname"></a>
# **DeleteClientCertAuthorityOcspTlsTrustedCommonName**
> SempMetaOnlyResponse DeleteClientCertAuthorityOcspTlsTrustedCommonName (string certAuthorityName, string ocspTlsTrustedCommonName)

Delete an OCSP Responder Trusted Common Name object.

Delete an OCSP Responder Trusted Common Name object. The deletion of instances of this object are synchronized to HA mates via config-sync.  When an OCSP override URL is configured, the OCSP responder will be required to sign the OCSP responses with certificates issued to these Trusted Common Names. A maximum of 8 common names can be configured as valid response signers.  A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been available since 2.19.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteClientCertAuthorityOcspTlsTrustedCommonNameExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new ClientCertAuthorityApi();
            var certAuthorityName = certAuthorityName_example;  // string | The name of the Certificate Authority.
            var ocspTlsTrustedCommonName = ocspTlsTrustedCommonName_example;  // string | The expected Trusted Common Name of the OCSP responder remote certificate.

            try
            {
                // Delete an OCSP Responder Trusted Common Name object.
                SempMetaOnlyResponse result = apiInstance.DeleteClientCertAuthorityOcspTlsTrustedCommonName(certAuthorityName, ocspTlsTrustedCommonName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ClientCertAuthorityApi.DeleteClientCertAuthorityOcspTlsTrustedCommonName: " + e.Message );
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
<a name="getclientcertauthorities"></a>
# **GetClientCertAuthorities**
> ClientCertAuthoritiesResponse GetClientCertAuthorities (int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Client Certificate Authority objects.

Get a list of Client Certificate Authority objects.  Clients can authenticate with the message broker over TLS by presenting a valid client certificate. The message broker authenticates the client certificate by constructing a full certificate chain (from the client certificate to intermediate CAs to a configured root CA). The intermediate CAs in this chain can be provided by the client, or configured in the message broker. The root CA must be configured on the message broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: certAuthorityName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.19.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetClientCertAuthoritiesExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new ClientCertAuthorityApi();
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Client Certificate Authority objects.
                ClientCertAuthoritiesResponse result = apiInstance.GetClientCertAuthorities(count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ClientCertAuthorityApi.GetClientCertAuthorities: " + e.Message );
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

[**ClientCertAuthoritiesResponse**](ClientCertAuthoritiesResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getclientcertauthority"></a>
# **GetClientCertAuthority**
> ClientCertAuthorityResponse GetClientCertAuthority (string certAuthorityName, string opaquePassword = null, List<string> select = null)

Get a Client Certificate Authority object.

Get a Client Certificate Authority object.  Clients can authenticate with the message broker over TLS by presenting a valid client certificate. The message broker authenticates the client certificate by constructing a full certificate chain (from the client certificate to intermediate CAs to a configured root CA). The intermediate CAs in this chain can be provided by the client, or configured in the message broker. The root CA must be configured on the message broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: certAuthorityName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.19.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetClientCertAuthorityExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new ClientCertAuthorityApi();
            var certAuthorityName = certAuthorityName_example;  // string | The name of the Certificate Authority.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Client Certificate Authority object.
                ClientCertAuthorityResponse result = apiInstance.GetClientCertAuthority(certAuthorityName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ClientCertAuthorityApi.GetClientCertAuthority: " + e.Message );
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

[**ClientCertAuthorityResponse**](ClientCertAuthorityResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getclientcertauthorityocsptlstrustedcommonname"></a>
# **GetClientCertAuthorityOcspTlsTrustedCommonName**
> ClientCertAuthorityOcspTlsTrustedCommonNameResponse GetClientCertAuthorityOcspTlsTrustedCommonName (string certAuthorityName, string ocspTlsTrustedCommonName, string opaquePassword = null, List<string> select = null)

Get an OCSP Responder Trusted Common Name object.

Get an OCSP Responder Trusted Common Name object.  When an OCSP override URL is configured, the OCSP responder will be required to sign the OCSP responses with certificates issued to these Trusted Common Names. A maximum of 8 common names can be configured as valid response signers.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: certAuthorityName|x||| ocspTlsTrustedCommonName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.19.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetClientCertAuthorityOcspTlsTrustedCommonNameExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new ClientCertAuthorityApi();
            var certAuthorityName = certAuthorityName_example;  // string | The name of the Certificate Authority.
            var ocspTlsTrustedCommonName = ocspTlsTrustedCommonName_example;  // string | The expected Trusted Common Name of the OCSP responder remote certificate.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get an OCSP Responder Trusted Common Name object.
                ClientCertAuthorityOcspTlsTrustedCommonNameResponse result = apiInstance.GetClientCertAuthorityOcspTlsTrustedCommonName(certAuthorityName, ocspTlsTrustedCommonName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ClientCertAuthorityApi.GetClientCertAuthorityOcspTlsTrustedCommonName: " + e.Message );
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

[**ClientCertAuthorityOcspTlsTrustedCommonNameResponse**](ClientCertAuthorityOcspTlsTrustedCommonNameResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getclientcertauthorityocsptlstrustedcommonnames"></a>
# **GetClientCertAuthorityOcspTlsTrustedCommonNames**
> ClientCertAuthorityOcspTlsTrustedCommonNamesResponse GetClientCertAuthorityOcspTlsTrustedCommonNames (string certAuthorityName, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of OCSP Responder Trusted Common Name objects.

Get a list of OCSP Responder Trusted Common Name objects.  When an OCSP override URL is configured, the OCSP responder will be required to sign the OCSP responses with certificates issued to these Trusted Common Names. A maximum of 8 common names can be configured as valid response signers.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: certAuthorityName|x||| ocspTlsTrustedCommonName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.19.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetClientCertAuthorityOcspTlsTrustedCommonNamesExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new ClientCertAuthorityApi();
            var certAuthorityName = certAuthorityName_example;  // string | The name of the Certificate Authority.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of OCSP Responder Trusted Common Name objects.
                ClientCertAuthorityOcspTlsTrustedCommonNamesResponse result = apiInstance.GetClientCertAuthorityOcspTlsTrustedCommonNames(certAuthorityName, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ClientCertAuthorityApi.GetClientCertAuthorityOcspTlsTrustedCommonNames: " + e.Message );
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

[**ClientCertAuthorityOcspTlsTrustedCommonNamesResponse**](ClientCertAuthorityOcspTlsTrustedCommonNamesResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replaceclientcertauthority"></a>
# **ReplaceClientCertAuthority**
> ClientCertAuthorityResponse ReplaceClientCertAuthority (ClientCertAuthority body, string certAuthorityName, string opaquePassword = null, List<string> select = null)

Replace a Client Certificate Authority object.

Replace a Client Certificate Authority object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Clients can authenticate with the message broker over TLS by presenting a valid client certificate. The message broker authenticates the client certificate by constructing a full certificate chain (from the client certificate to intermediate CAs to a configured root CA). The intermediate CAs in this chain can be provided by the client, or configured in the message broker. The root CA must be configured on the message broker.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- certAuthorityName|x||x||||| crlUrl||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- ClientCertAuthority|crlDayList|crlTimeList| ClientCertAuthority|crlTimeList|crlDayList|    A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been available since 2.19.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceClientCertAuthorityExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new ClientCertAuthorityApi();
            var body = new ClientCertAuthority(); // ClientCertAuthority | The Client Certificate Authority object's attributes.
            var certAuthorityName = certAuthorityName_example;  // string | The name of the Certificate Authority.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Client Certificate Authority object.
                ClientCertAuthorityResponse result = apiInstance.ReplaceClientCertAuthority(body, certAuthorityName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ClientCertAuthorityApi.ReplaceClientCertAuthority: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**ClientCertAuthority**](ClientCertAuthority.md)| The Client Certificate Authority object&#x27;s attributes. | 
 **certAuthorityName** | **string**| The name of the Certificate Authority. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**ClientCertAuthorityResponse**](ClientCertAuthorityResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updateclientcertauthority"></a>
# **UpdateClientCertAuthority**
> ClientCertAuthorityResponse UpdateClientCertAuthority (ClientCertAuthority body, string certAuthorityName, string opaquePassword = null, List<string> select = null)

Update a Client Certificate Authority object.

Update a Client Certificate Authority object. Any attribute missing from the request will be left unchanged.  Clients can authenticate with the message broker over TLS by presenting a valid client certificate. The message broker authenticates the client certificate by constructing a full certificate chain (from the client certificate to intermediate CAs to a configured root CA). The intermediate CAs in this chain can be provided by the client, or configured in the message broker. The root CA must be configured on the message broker.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- certAuthorityName|x|x||||| crlUrl|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- ClientCertAuthority|crlDayList|crlTimeList| ClientCertAuthority|crlTimeList|crlDayList|    A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been available since 2.19.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateClientCertAuthorityExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new ClientCertAuthorityApi();
            var body = new ClientCertAuthority(); // ClientCertAuthority | The Client Certificate Authority object's attributes.
            var certAuthorityName = certAuthorityName_example;  // string | The name of the Certificate Authority.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Client Certificate Authority object.
                ClientCertAuthorityResponse result = apiInstance.UpdateClientCertAuthority(body, certAuthorityName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ClientCertAuthorityApi.UpdateClientCertAuthority: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**ClientCertAuthority**](ClientCertAuthority.md)| The Client Certificate Authority object&#x27;s attributes. | 
 **certAuthorityName** | **string**| The name of the Certificate Authority. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**ClientCertAuthorityResponse**](ClientCertAuthorityResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
