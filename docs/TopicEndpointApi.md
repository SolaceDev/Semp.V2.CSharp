# Semp.V2.CSharp.Api.TopicEndpointApi

All URIs are relative to *http://www.solace.com/SEMP/v2/config*

Method | HTTP request | Description
------------- | ------------- | -------------
[**CreateMsgVpnTopicEndpoint**](TopicEndpointApi.md#createmsgvpntopicendpoint) | **POST** /msgVpns/{msgVpnName}/topicEndpoints | Create a Topic Endpoint object.
[**DeleteMsgVpnTopicEndpoint**](TopicEndpointApi.md#deletemsgvpntopicendpoint) | **DELETE** /msgVpns/{msgVpnName}/topicEndpoints/{topicEndpointName} | Delete a Topic Endpoint object.
[**GetMsgVpnTopicEndpoint**](TopicEndpointApi.md#getmsgvpntopicendpoint) | **GET** /msgVpns/{msgVpnName}/topicEndpoints/{topicEndpointName} | Get a Topic Endpoint object.
[**GetMsgVpnTopicEndpoints**](TopicEndpointApi.md#getmsgvpntopicendpoints) | **GET** /msgVpns/{msgVpnName}/topicEndpoints | Get a list of Topic Endpoint objects.
[**ReplaceMsgVpnTopicEndpoint**](TopicEndpointApi.md#replacemsgvpntopicendpoint) | **PUT** /msgVpns/{msgVpnName}/topicEndpoints/{topicEndpointName} | Replace a Topic Endpoint object.
[**UpdateMsgVpnTopicEndpoint**](TopicEndpointApi.md#updatemsgvpntopicendpoint) | **PATCH** /msgVpns/{msgVpnName}/topicEndpoints/{topicEndpointName} | Update a Topic Endpoint object.

<a name="createmsgvpntopicendpoint"></a>
# **CreateMsgVpnTopicEndpoint**
> MsgVpnTopicEndpointResponse CreateMsgVpnTopicEndpoint (MsgVpnTopicEndpoint body, string msgVpnName, string opaquePassword = null, List<string> select = null)

Create a Topic Endpoint object.

Create a Topic Endpoint object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Topic Endpoint attracts messages published to a topic for which the Topic Endpoint has a matching topic subscription. The topic subscription for the Topic Endpoint is specified in the client request to bind a Flow to that Topic Endpoint. Queues are significantly more flexible than Topic Endpoints and are the recommended approach for most applications. The use of Topic Endpoints should be restricted to JMS applications.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| topicEndpointName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.1.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnTopicEndpointExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TopicEndpointApi();
            var body = new MsgVpnTopicEndpoint(); // MsgVpnTopicEndpoint | The Topic Endpoint object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Topic Endpoint object.
                MsgVpnTopicEndpointResponse result = apiInstance.CreateMsgVpnTopicEndpoint(body, msgVpnName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TopicEndpointApi.CreateMsgVpnTopicEndpoint: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnTopicEndpoint**](MsgVpnTopicEndpoint.md)| The Topic Endpoint object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTopicEndpointResponse**](MsgVpnTopicEndpointResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpntopicendpoint"></a>
# **DeleteMsgVpnTopicEndpoint**
> SempMetaOnlyResponse DeleteMsgVpnTopicEndpoint (string msgVpnName, string topicEndpointName)

Delete a Topic Endpoint object.

Delete a Topic Endpoint object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Topic Endpoint attracts messages published to a topic for which the Topic Endpoint has a matching topic subscription. The topic subscription for the Topic Endpoint is specified in the client request to bind a Flow to that Topic Endpoint. Queues are significantly more flexible than Topic Endpoints and are the recommended approach for most applications. The use of Topic Endpoints should be restricted to JMS applications.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.1.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnTopicEndpointExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TopicEndpointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var topicEndpointName = topicEndpointName_example;  // string | The name of the Topic Endpoint.

            try
            {
                // Delete a Topic Endpoint object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnTopicEndpoint(msgVpnName, topicEndpointName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TopicEndpointApi.DeleteMsgVpnTopicEndpoint: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **topicEndpointName** | **string**| The name of the Topic Endpoint. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpntopicendpoint"></a>
# **GetMsgVpnTopicEndpoint**
> MsgVpnTopicEndpointResponse GetMsgVpnTopicEndpoint (string msgVpnName, string topicEndpointName, string opaquePassword = null, List<string> select = null)

Get a Topic Endpoint object.

Get a Topic Endpoint object.  A Topic Endpoint attracts messages published to a topic for which the Topic Endpoint has a matching topic subscription. The topic subscription for the Topic Endpoint is specified in the client request to bind a Flow to that Topic Endpoint. Queues are significantly more flexible than Topic Endpoints and are the recommended approach for most applications. The use of Topic Endpoints should be restricted to JMS applications.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| topicEndpointName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.1.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnTopicEndpointExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TopicEndpointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var topicEndpointName = topicEndpointName_example;  // string | The name of the Topic Endpoint.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Topic Endpoint object.
                MsgVpnTopicEndpointResponse result = apiInstance.GetMsgVpnTopicEndpoint(msgVpnName, topicEndpointName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TopicEndpointApi.GetMsgVpnTopicEndpoint: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **topicEndpointName** | **string**| The name of the Topic Endpoint. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTopicEndpointResponse**](MsgVpnTopicEndpointResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpntopicendpoints"></a>
# **GetMsgVpnTopicEndpoints**
> MsgVpnTopicEndpointsResponse GetMsgVpnTopicEndpoints (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Topic Endpoint objects.

Get a list of Topic Endpoint objects.  A Topic Endpoint attracts messages published to a topic for which the Topic Endpoint has a matching topic subscription. The topic subscription for the Topic Endpoint is specified in the client request to bind a Flow to that Topic Endpoint. Queues are significantly more flexible than Topic Endpoints and are the recommended approach for most applications. The use of Topic Endpoints should be restricted to JMS applications.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| topicEndpointName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.1.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnTopicEndpointsExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TopicEndpointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Topic Endpoint objects.
                MsgVpnTopicEndpointsResponse result = apiInstance.GetMsgVpnTopicEndpoints(msgVpnName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TopicEndpointApi.GetMsgVpnTopicEndpoints: " + e.Message );
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

[**MsgVpnTopicEndpointsResponse**](MsgVpnTopicEndpointsResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacemsgvpntopicendpoint"></a>
# **ReplaceMsgVpnTopicEndpoint**
> MsgVpnTopicEndpointResponse ReplaceMsgVpnTopicEndpoint (MsgVpnTopicEndpoint body, string msgVpnName, string topicEndpointName, string opaquePassword = null, List<string> select = null)

Replace a Topic Endpoint object.

Replace a Topic Endpoint object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Topic Endpoint attracts messages published to a topic for which the Topic Endpoint has a matching topic subscription. The topic subscription for the Topic Endpoint is specified in the client request to bind a Flow to that Topic Endpoint. Queues are significantly more flexible than Topic Endpoints and are the recommended approach for most applications. The use of Topic Endpoints should be restricted to JMS applications.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- accessType||||||x|| msgVpnName|x||x||||| owner||||||x|| permission||||||x|| redeliveryDelayEnabled||||||x|| redeliveryDelayInitialInterval||||||x|| redeliveryDelayMaxInterval||||||x|| redeliveryDelayMultiplier||||||x|| rejectMsgToSenderOnDiscardBehavior||||||x|| respectMsgPriorityEnabled||||||x|| topicEndpointName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.1.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceMsgVpnTopicEndpointExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TopicEndpointApi();
            var body = new MsgVpnTopicEndpoint(); // MsgVpnTopicEndpoint | The Topic Endpoint object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var topicEndpointName = topicEndpointName_example;  // string | The name of the Topic Endpoint.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Topic Endpoint object.
                MsgVpnTopicEndpointResponse result = apiInstance.ReplaceMsgVpnTopicEndpoint(body, msgVpnName, topicEndpointName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TopicEndpointApi.ReplaceMsgVpnTopicEndpoint: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnTopicEndpoint**](MsgVpnTopicEndpoint.md)| The Topic Endpoint object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **topicEndpointName** | **string**| The name of the Topic Endpoint. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTopicEndpointResponse**](MsgVpnTopicEndpointResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatemsgvpntopicendpoint"></a>
# **UpdateMsgVpnTopicEndpoint**
> MsgVpnTopicEndpointResponse UpdateMsgVpnTopicEndpoint (MsgVpnTopicEndpoint body, string msgVpnName, string topicEndpointName, string opaquePassword = null, List<string> select = null)

Update a Topic Endpoint object.

Update a Topic Endpoint object. Any attribute missing from the request will be left unchanged.  A Topic Endpoint attracts messages published to a topic for which the Topic Endpoint has a matching topic subscription. The topic subscription for the Topic Endpoint is specified in the client request to bind a Flow to that Topic Endpoint. Queues are significantly more flexible than Topic Endpoints and are the recommended approach for most applications. The use of Topic Endpoints should be restricted to JMS applications.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- accessType|||||x|| msgVpnName|x|x||||| owner|||||x|| permission|||||x|| redeliveryDelayEnabled|||||x|| redeliveryDelayInitialInterval|||||x|| redeliveryDelayMaxInterval|||||x|| redeliveryDelayMultiplier|||||x|| rejectMsgToSenderOnDiscardBehavior|||||x|| respectMsgPriorityEnabled|||||x|| topicEndpointName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.1.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateMsgVpnTopicEndpointExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TopicEndpointApi();
            var body = new MsgVpnTopicEndpoint(); // MsgVpnTopicEndpoint | The Topic Endpoint object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var topicEndpointName = topicEndpointName_example;  // string | The name of the Topic Endpoint.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Topic Endpoint object.
                MsgVpnTopicEndpointResponse result = apiInstance.UpdateMsgVpnTopicEndpoint(body, msgVpnName, topicEndpointName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TopicEndpointApi.UpdateMsgVpnTopicEndpoint: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnTopicEndpoint**](MsgVpnTopicEndpoint.md)| The Topic Endpoint object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **topicEndpointName** | **string**| The name of the Topic Endpoint. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTopicEndpointResponse**](MsgVpnTopicEndpointResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
