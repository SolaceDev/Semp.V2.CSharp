# Semp.V2.CSharp.Api.VirtualHostnameApi

All URIs are relative to *http://www.solace.com/SEMP/v2/config*

Method | HTTP request | Description
------------- | ------------- | -------------
[**CreateVirtualHostname**](VirtualHostnameApi.md#createvirtualhostname) | **POST** /virtualHostnames | Create a Virtual Hostname object.
[**DeleteVirtualHostname**](VirtualHostnameApi.md#deletevirtualhostname) | **DELETE** /virtualHostnames/{virtualHostname} | Delete a Virtual Hostname object.
[**GetVirtualHostname**](VirtualHostnameApi.md#getvirtualhostname) | **GET** /virtualHostnames/{virtualHostname} | Get a Virtual Hostname object.
[**GetVirtualHostnames**](VirtualHostnameApi.md#getvirtualhostnames) | **GET** /virtualHostnames | Get a list of Virtual Hostname objects.
[**ReplaceVirtualHostname**](VirtualHostnameApi.md#replacevirtualhostname) | **PUT** /virtualHostnames/{virtualHostname} | Replace a Virtual Hostname object.
[**UpdateVirtualHostname**](VirtualHostnameApi.md#updatevirtualhostname) | **PATCH** /virtualHostnames/{virtualHostname} | Update a Virtual Hostname object.

<a name="createvirtualhostname"></a>
# **CreateVirtualHostname**
> VirtualHostnameResponse CreateVirtualHostname (VirtualHostname body, string opaquePassword = null, List<string> select = null)

Create a Virtual Hostname object.

Create a Virtual Hostname object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A Virtual Hostname is a provisioned object on a message broker that contains a Virtual Hostname to Message VPN mapping.  Clients which connect to a global (as opposed to per Message VPN) port and provides this hostname will be directed to its corresponding Message VPN. A case-insentive match is performed on the full client-provided hostname against the configured virtual-hostname.  This mechanism is only supported for hostnames provided through the Server Name Indication (SNI) extension of TLS.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: virtualHostname|x|x||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.17.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateVirtualHostnameExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new VirtualHostnameApi();
            var body = new VirtualHostname(); // VirtualHostname | The Virtual Hostname object's attributes.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Virtual Hostname object.
                VirtualHostnameResponse result = apiInstance.CreateVirtualHostname(body, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling VirtualHostnameApi.CreateVirtualHostname: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**VirtualHostname**](VirtualHostname.md)| The Virtual Hostname object&#x27;s attributes. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**VirtualHostnameResponse**](VirtualHostnameResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletevirtualhostname"></a>
# **DeleteVirtualHostname**
> SempMetaOnlyResponse DeleteVirtualHostname (string virtualHostname)

Delete a Virtual Hostname object.

Delete a Virtual Hostname object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A Virtual Hostname is a provisioned object on a message broker that contains a Virtual Hostname to Message VPN mapping.  Clients which connect to a global (as opposed to per Message VPN) port and provides this hostname will be directed to its corresponding Message VPN. A case-insentive match is performed on the full client-provided hostname against the configured virtual-hostname.  This mechanism is only supported for hostnames provided through the Server Name Indication (SNI) extension of TLS.  A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.17.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteVirtualHostnameExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new VirtualHostnameApi();
            var virtualHostname = virtualHostname_example;  // string | The virtual hostname.

            try
            {
                // Delete a Virtual Hostname object.
                SempMetaOnlyResponse result = apiInstance.DeleteVirtualHostname(virtualHostname);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling VirtualHostnameApi.DeleteVirtualHostname: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **virtualHostname** | **string**| The virtual hostname. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getvirtualhostname"></a>
# **GetVirtualHostname**
> VirtualHostnameResponse GetVirtualHostname (string virtualHostname, string opaquePassword = null, List<string> select = null)

Get a Virtual Hostname object.

Get a Virtual Hostname object.  A Virtual Hostname is a provisioned object on a message broker that contains a Virtual Hostname to Message VPN mapping.  Clients which connect to a global (as opposed to per Message VPN) port and provides this hostname will be directed to its corresponding Message VPN. A case-insentive match is performed on the full client-provided hostname against the configured virtual-hostname.  This mechanism is only supported for hostnames provided through the Server Name Indication (SNI) extension of TLS.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: virtualHostname|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.17.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetVirtualHostnameExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new VirtualHostnameApi();
            var virtualHostname = virtualHostname_example;  // string | The virtual hostname.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Virtual Hostname object.
                VirtualHostnameResponse result = apiInstance.GetVirtualHostname(virtualHostname, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling VirtualHostnameApi.GetVirtualHostname: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **virtualHostname** | **string**| The virtual hostname. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**VirtualHostnameResponse**](VirtualHostnameResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getvirtualhostnames"></a>
# **GetVirtualHostnames**
> VirtualHostnamesResponse GetVirtualHostnames (int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Virtual Hostname objects.

Get a list of Virtual Hostname objects.  A Virtual Hostname is a provisioned object on a message broker that contains a Virtual Hostname to Message VPN mapping.  Clients which connect to a global (as opposed to per Message VPN) port and provides this hostname will be directed to its corresponding Message VPN. A case-insentive match is performed on the full client-provided hostname against the configured virtual-hostname.  This mechanism is only supported for hostnames provided through the Server Name Indication (SNI) extension of TLS.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: virtualHostname|x|||    A SEMP client authorized with a minimum access scope/level of \"global/read-only\" is required to perform this operation.  This has been available since 2.17.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetVirtualHostnamesExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new VirtualHostnameApi();
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Virtual Hostname objects.
                VirtualHostnamesResponse result = apiInstance.GetVirtualHostnames(count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling VirtualHostnameApi.GetVirtualHostnames: " + e.Message );
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

[**VirtualHostnamesResponse**](VirtualHostnamesResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacevirtualhostname"></a>
# **ReplaceVirtualHostname**
> VirtualHostnameResponse ReplaceVirtualHostname (VirtualHostname body, string virtualHostname, string opaquePassword = null, List<string> select = null)

Replace a Virtual Hostname object.

Replace a Virtual Hostname object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Virtual Hostname is a provisioned object on a message broker that contains a Virtual Hostname to Message VPN mapping.  Clients which connect to a global (as opposed to per Message VPN) port and provides this hostname will be directed to its corresponding Message VPN. A case-insentive match is performed on the full client-provided hostname against the configured virtual-hostname.  This mechanism is only supported for hostnames provided through the Server Name Indication (SNI) extension of TLS.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- virtualHostname|x||x|||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.17.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceVirtualHostnameExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new VirtualHostnameApi();
            var body = new VirtualHostname(); // VirtualHostname | The Virtual Hostname object's attributes.
            var virtualHostname = virtualHostname_example;  // string | The virtual hostname.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Virtual Hostname object.
                VirtualHostnameResponse result = apiInstance.ReplaceVirtualHostname(body, virtualHostname, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling VirtualHostnameApi.ReplaceVirtualHostname: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**VirtualHostname**](VirtualHostname.md)| The Virtual Hostname object&#x27;s attributes. | 
 **virtualHostname** | **string**| The virtual hostname. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**VirtualHostnameResponse**](VirtualHostnameResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatevirtualhostname"></a>
# **UpdateVirtualHostname**
> VirtualHostnameResponse UpdateVirtualHostname (VirtualHostname body, string virtualHostname, string opaquePassword = null, List<string> select = null)

Update a Virtual Hostname object.

Update a Virtual Hostname object. Any attribute missing from the request will be left unchanged.  A Virtual Hostname is a provisioned object on a message broker that contains a Virtual Hostname to Message VPN mapping.  Clients which connect to a global (as opposed to per Message VPN) port and provides this hostname will be directed to its corresponding Message VPN. A case-insentive match is performed on the full client-provided hostname against the configured virtual-hostname.  This mechanism is only supported for hostnames provided through the Server Name Indication (SNI) extension of TLS.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- virtualHostname|x|x|||||    A SEMP client authorized with a minimum access scope/level of \"global/read-write\" is required to perform this operation.  This has been available since 2.17.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateVirtualHostnameExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new VirtualHostnameApi();
            var body = new VirtualHostname(); // VirtualHostname | The Virtual Hostname object's attributes.
            var virtualHostname = virtualHostname_example;  // string | The virtual hostname.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Virtual Hostname object.
                VirtualHostnameResponse result = apiInstance.UpdateVirtualHostname(body, virtualHostname, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling VirtualHostnameApi.UpdateVirtualHostname: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**VirtualHostname**](VirtualHostname.md)| The Virtual Hostname object&#x27;s attributes. | 
 **virtualHostname** | **string**| The virtual hostname. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**VirtualHostnameResponse**](VirtualHostnameResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
