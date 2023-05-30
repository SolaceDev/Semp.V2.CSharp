# Semp.V2.CSharp.Api.TopicEndpointTemplateApi

All URIs are relative to *http://www.solace.com/SEMP/v2/config*

Method | HTTP request | Description
------------- | ------------- | -------------
[**CreateMsgVpnTopicEndpointTemplate**](TopicEndpointTemplateApi.md#createmsgvpntopicendpointtemplate) | **POST** /msgVpns/{msgVpnName}/topicEndpointTemplates | Create a Topic Endpoint Template object.
[**DeleteMsgVpnTopicEndpointTemplate**](TopicEndpointTemplateApi.md#deletemsgvpntopicendpointtemplate) | **DELETE** /msgVpns/{msgVpnName}/topicEndpointTemplates/{topicEndpointTemplateName} | Delete a Topic Endpoint Template object.
[**GetMsgVpnTopicEndpointTemplate**](TopicEndpointTemplateApi.md#getmsgvpntopicendpointtemplate) | **GET** /msgVpns/{msgVpnName}/topicEndpointTemplates/{topicEndpointTemplateName} | Get a Topic Endpoint Template object.
[**GetMsgVpnTopicEndpointTemplates**](TopicEndpointTemplateApi.md#getmsgvpntopicendpointtemplates) | **GET** /msgVpns/{msgVpnName}/topicEndpointTemplates | Get a list of Topic Endpoint Template objects.
[**ReplaceMsgVpnTopicEndpointTemplate**](TopicEndpointTemplateApi.md#replacemsgvpntopicendpointtemplate) | **PUT** /msgVpns/{msgVpnName}/topicEndpointTemplates/{topicEndpointTemplateName} | Replace a Topic Endpoint Template object.
[**UpdateMsgVpnTopicEndpointTemplate**](TopicEndpointTemplateApi.md#updatemsgvpntopicendpointtemplate) | **PATCH** /msgVpns/{msgVpnName}/topicEndpointTemplates/{topicEndpointTemplateName} | Update a Topic Endpoint Template object.

<a name="createmsgvpntopicendpointtemplate"></a>
# **CreateMsgVpnTopicEndpointTemplate**
> MsgVpnTopicEndpointTemplateResponse CreateMsgVpnTopicEndpointTemplate (MsgVpnTopicEndpointTemplate body, string msgVpnName, string opaquePassword = null, List<string> select = null)

Create a Topic Endpoint Template object.

Create a Topic Endpoint Template object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Topic Endpoint Template provides a mechanism for specifying the initial state for client created topic endpoints.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| topicEndpointTemplateName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.14.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnTopicEndpointTemplateExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TopicEndpointTemplateApi();
            var body = new MsgVpnTopicEndpointTemplate(); // MsgVpnTopicEndpointTemplate | The Topic Endpoint Template object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Topic Endpoint Template object.
                MsgVpnTopicEndpointTemplateResponse result = apiInstance.CreateMsgVpnTopicEndpointTemplate(body, msgVpnName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TopicEndpointTemplateApi.CreateMsgVpnTopicEndpointTemplate: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnTopicEndpointTemplate**](MsgVpnTopicEndpointTemplate.md)| The Topic Endpoint Template object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTopicEndpointTemplateResponse**](MsgVpnTopicEndpointTemplateResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpntopicendpointtemplate"></a>
# **DeleteMsgVpnTopicEndpointTemplate**
> SempMetaOnlyResponse DeleteMsgVpnTopicEndpointTemplate (string msgVpnName, string topicEndpointTemplateName)

Delete a Topic Endpoint Template object.

Delete a Topic Endpoint Template object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Topic Endpoint Template provides a mechanism for specifying the initial state for client created topic endpoints.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.14.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnTopicEndpointTemplateExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TopicEndpointTemplateApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var topicEndpointTemplateName = topicEndpointTemplateName_example;  // string | The name of the Topic Endpoint Template.

            try
            {
                // Delete a Topic Endpoint Template object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnTopicEndpointTemplate(msgVpnName, topicEndpointTemplateName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TopicEndpointTemplateApi.DeleteMsgVpnTopicEndpointTemplate: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **topicEndpointTemplateName** | **string**| The name of the Topic Endpoint Template. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpntopicendpointtemplate"></a>
# **GetMsgVpnTopicEndpointTemplate**
> MsgVpnTopicEndpointTemplateResponse GetMsgVpnTopicEndpointTemplate (string msgVpnName, string topicEndpointTemplateName, string opaquePassword = null, List<string> select = null)

Get a Topic Endpoint Template object.

Get a Topic Endpoint Template object.  A Topic Endpoint Template provides a mechanism for specifying the initial state for client created topic endpoints.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| topicEndpointTemplateName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.14.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnTopicEndpointTemplateExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TopicEndpointTemplateApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var topicEndpointTemplateName = topicEndpointTemplateName_example;  // string | The name of the Topic Endpoint Template.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Topic Endpoint Template object.
                MsgVpnTopicEndpointTemplateResponse result = apiInstance.GetMsgVpnTopicEndpointTemplate(msgVpnName, topicEndpointTemplateName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TopicEndpointTemplateApi.GetMsgVpnTopicEndpointTemplate: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **topicEndpointTemplateName** | **string**| The name of the Topic Endpoint Template. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTopicEndpointTemplateResponse**](MsgVpnTopicEndpointTemplateResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpntopicendpointtemplates"></a>
# **GetMsgVpnTopicEndpointTemplates**
> MsgVpnTopicEndpointTemplatesResponse GetMsgVpnTopicEndpointTemplates (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Topic Endpoint Template objects.

Get a list of Topic Endpoint Template objects.  A Topic Endpoint Template provides a mechanism for specifying the initial state for client created topic endpoints.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| topicEndpointTemplateName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.14.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnTopicEndpointTemplatesExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TopicEndpointTemplateApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Topic Endpoint Template objects.
                MsgVpnTopicEndpointTemplatesResponse result = apiInstance.GetMsgVpnTopicEndpointTemplates(msgVpnName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TopicEndpointTemplateApi.GetMsgVpnTopicEndpointTemplates: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTopicEndpointTemplatesResponse**](MsgVpnTopicEndpointTemplatesResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacemsgvpntopicendpointtemplate"></a>
# **ReplaceMsgVpnTopicEndpointTemplate**
> MsgVpnTopicEndpointTemplateResponse ReplaceMsgVpnTopicEndpointTemplate (MsgVpnTopicEndpointTemplate body, string msgVpnName, string topicEndpointTemplateName, string opaquePassword = null, List<string> select = null)

Replace a Topic Endpoint Template object.

Replace a Topic Endpoint Template object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Topic Endpoint Template provides a mechanism for specifying the initial state for client created topic endpoints.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| rejectMsgToSenderOnDiscardBehavior||||||x|| topicEndpointTemplateName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.14.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceMsgVpnTopicEndpointTemplateExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TopicEndpointTemplateApi();
            var body = new MsgVpnTopicEndpointTemplate(); // MsgVpnTopicEndpointTemplate | The Topic Endpoint Template object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var topicEndpointTemplateName = topicEndpointTemplateName_example;  // string | The name of the Topic Endpoint Template.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Topic Endpoint Template object.
                MsgVpnTopicEndpointTemplateResponse result = apiInstance.ReplaceMsgVpnTopicEndpointTemplate(body, msgVpnName, topicEndpointTemplateName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TopicEndpointTemplateApi.ReplaceMsgVpnTopicEndpointTemplate: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnTopicEndpointTemplate**](MsgVpnTopicEndpointTemplate.md)| The Topic Endpoint Template object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **topicEndpointTemplateName** | **string**| The name of the Topic Endpoint Template. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTopicEndpointTemplateResponse**](MsgVpnTopicEndpointTemplateResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatemsgvpntopicendpointtemplate"></a>
# **UpdateMsgVpnTopicEndpointTemplate**
> MsgVpnTopicEndpointTemplateResponse UpdateMsgVpnTopicEndpointTemplate (MsgVpnTopicEndpointTemplate body, string msgVpnName, string topicEndpointTemplateName, string opaquePassword = null, List<string> select = null)

Update a Topic Endpoint Template object.

Update a Topic Endpoint Template object. Any attribute missing from the request will be left unchanged.  A Topic Endpoint Template provides a mechanism for specifying the initial state for client created topic endpoints.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| rejectMsgToSenderOnDiscardBehavior|||||x|| topicEndpointTemplateName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.14.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateMsgVpnTopicEndpointTemplateExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TopicEndpointTemplateApi();
            var body = new MsgVpnTopicEndpointTemplate(); // MsgVpnTopicEndpointTemplate | The Topic Endpoint Template object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var topicEndpointTemplateName = topicEndpointTemplateName_example;  // string | The name of the Topic Endpoint Template.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Topic Endpoint Template object.
                MsgVpnTopicEndpointTemplateResponse result = apiInstance.UpdateMsgVpnTopicEndpointTemplate(body, msgVpnName, topicEndpointTemplateName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TopicEndpointTemplateApi.UpdateMsgVpnTopicEndpointTemplate: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnTopicEndpointTemplate**](MsgVpnTopicEndpointTemplate.md)| The Topic Endpoint Template object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **topicEndpointTemplateName** | **string**| The name of the Topic Endpoint Template. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTopicEndpointTemplateResponse**](MsgVpnTopicEndpointTemplateResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
