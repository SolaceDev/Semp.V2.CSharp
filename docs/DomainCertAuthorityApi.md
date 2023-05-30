# Semp.V2.CSharp.Api.DomainCertAuthorityApi

All URIs are relative to *http://www.solace.com/SEMP/v2/config*

Method | HTTP request | Description
------------- | ------------- | -------------
[**CreateDomainCertAuthority**](DomainCertAuthorityApi.md#createdomaincertauthority) | **POST** /domainCertAuthorities | Create a Domain Certificate Authority object.
[**DeleteDomainCertAuthority**](DomainCertAuthorityApi.md#deletedomaincertauthority) | **DELETE** /domainCertAuthorities/{certAuthorityName} | Delete a Domain Certificate Authority object.
[**GetDomainCertAuthorities**](DomainCertAuthorityApi.md#getdomaincertauthorities) | **GET** /domainCertAuthorities | Get a list of Domain Certificate Authority objects.
[**GetDomainCertAuthority**](DomainCertAuthorityApi.md#getdomaincertauthority) | **GET** /domainCertAuthorities/{certAuthorityName} | Get a Domain Certificate Authority object.
[**ReplaceDomainCertAuthority**](DomainCertAuthorityApi.md#replacedomaincertauthority) | **PUT** /domainCertAuthorities/{certAuthorityName} | Replace a Domain Certificate Authority object.
[**UpdateDomainCertAuthority**](DomainCertAuthorityApi.md#updatedomaincertauthority) | **PATCH** /domainCertAuthorities/{certAuthorityName} | Update a Domain Certificate Authority object.

<a name="createdomaincertauthority"></a>
# **CreateDomainCertAuthority**
> DomainCertAuthorityResponse CreateDomainCertAuthority (DomainCertAuthority body, string opaquePassword = null, List<string> select = null)

Create a Domain Certificate Authority object.

Create a Domain Certificate Authority object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Certificate Authorities trusted for domain verification.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: certAuthorityName|x|x||||    A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been available since 2.19.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateDomainCertAuthorityExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DomainCertAuthorityApi();
            var body = new DomainCertAuthority(); // DomainCertAuthority | The Domain Certificate Authority object's attributes.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Domain Certificate Authority object.
                DomainCertAuthorityResponse result = apiInstance.CreateDomainCertAuthority(body, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DomainCertAuthorityApi.CreateDomainCertAuthority: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**DomainCertAuthority**](DomainCertAuthority.md)| The Domain Certificate Authority object&#x27;s attributes. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DomainCertAuthorityResponse**](DomainCertAuthorityResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletedomaincertauthority"></a>
# **DeleteDomainCertAuthority**
> SempMetaOnlyResponse DeleteDomainCertAuthority (string certAuthorityName)

Delete a Domain Certificate Authority object.

Delete a Domain Certificate Authority object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Certificate Authorities trusted for domain verification.  A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been available since 2.19.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteDomainCertAuthorityExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DomainCertAuthorityApi();
            var certAuthorityName = certAuthorityName_example;  // string | The name of the Certificate Authority.

            try
            {
                // Delete a Domain Certificate Authority object.
                SempMetaOnlyResponse result = apiInstance.DeleteDomainCertAuthority(certAuthorityName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DomainCertAuthorityApi.DeleteDomainCertAuthority: " + e.Message );
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
<a name="getdomaincertauthorities"></a>
# **GetDomainCertAuthorities**
> DomainCertAuthoritiesResponse GetDomainCertAuthorities (int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Domain Certificate Authority objects.

Get a list of Domain Certificate Authority objects.  Certificate Authorities trusted for domain verification.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: certAuthorityName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.19.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetDomainCertAuthoritiesExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DomainCertAuthorityApi();
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Domain Certificate Authority objects.
                DomainCertAuthoritiesResponse result = apiInstance.GetDomainCertAuthorities(count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DomainCertAuthorityApi.GetDomainCertAuthorities: " + e.Message );
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

[**DomainCertAuthoritiesResponse**](DomainCertAuthoritiesResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getdomaincertauthority"></a>
# **GetDomainCertAuthority**
> DomainCertAuthorityResponse GetDomainCertAuthority (string certAuthorityName, string opaquePassword = null, List<string> select = null)

Get a Domain Certificate Authority object.

Get a Domain Certificate Authority object.  Certificate Authorities trusted for domain verification.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: certAuthorityName|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.19.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetDomainCertAuthorityExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DomainCertAuthorityApi();
            var certAuthorityName = certAuthorityName_example;  // string | The name of the Certificate Authority.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Domain Certificate Authority object.
                DomainCertAuthorityResponse result = apiInstance.GetDomainCertAuthority(certAuthorityName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DomainCertAuthorityApi.GetDomainCertAuthority: " + e.Message );
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

[**DomainCertAuthorityResponse**](DomainCertAuthorityResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacedomaincertauthority"></a>
# **ReplaceDomainCertAuthority**
> DomainCertAuthorityResponse ReplaceDomainCertAuthority (DomainCertAuthority body, string certAuthorityName, string opaquePassword = null, List<string> select = null)

Replace a Domain Certificate Authority object.

Replace a Domain Certificate Authority object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Certificate Authorities trusted for domain verification.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- certAuthorityName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been available since 2.19.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceDomainCertAuthorityExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DomainCertAuthorityApi();
            var body = new DomainCertAuthority(); // DomainCertAuthority | The Domain Certificate Authority object's attributes.
            var certAuthorityName = certAuthorityName_example;  // string | The name of the Certificate Authority.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Domain Certificate Authority object.
                DomainCertAuthorityResponse result = apiInstance.ReplaceDomainCertAuthority(body, certAuthorityName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DomainCertAuthorityApi.ReplaceDomainCertAuthority: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**DomainCertAuthority**](DomainCertAuthority.md)| The Domain Certificate Authority object&#x27;s attributes. | 
 **certAuthorityName** | **string**| The name of the Certificate Authority. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DomainCertAuthorityResponse**](DomainCertAuthorityResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatedomaincertauthority"></a>
# **UpdateDomainCertAuthority**
> DomainCertAuthorityResponse UpdateDomainCertAuthority (DomainCertAuthority body, string certAuthorityName, string opaquePassword = null, List<string> select = null)

Update a Domain Certificate Authority object.

Update a Domain Certificate Authority object. Any attribute missing from the request will be left unchanged.  Certificate Authorities trusted for domain verification.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- certAuthorityName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \"global/admin\" is required to perform this operation.  This has been available since 2.19.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateDomainCertAuthorityExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DomainCertAuthorityApi();
            var body = new DomainCertAuthority(); // DomainCertAuthority | The Domain Certificate Authority object's attributes.
            var certAuthorityName = certAuthorityName_example;  // string | The name of the Certificate Authority.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Domain Certificate Authority object.
                DomainCertAuthorityResponse result = apiInstance.UpdateDomainCertAuthority(body, certAuthorityName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DomainCertAuthorityApi.UpdateDomainCertAuthority: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**DomainCertAuthority**](DomainCertAuthority.md)| The Domain Certificate Authority object&#x27;s attributes. | 
 **certAuthorityName** | **string**| The name of the Certificate Authority. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**DomainCertAuthorityResponse**](DomainCertAuthorityResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
