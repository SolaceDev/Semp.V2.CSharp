# Semp.V2.CSharp.Api.ReplicatedTopicApi

All URIs are relative to *http://www.solace.com/SEMP/v2/config*

Method | HTTP request | Description
------------- | ------------- | -------------
[**CreateMsgVpnReplicatedTopic**](ReplicatedTopicApi.md#createmsgvpnreplicatedtopic) | **POST** /msgVpns/{msgVpnName}/replicatedTopics | Create a Replicated Topic object.
[**DeleteMsgVpnReplicatedTopic**](ReplicatedTopicApi.md#deletemsgvpnreplicatedtopic) | **DELETE** /msgVpns/{msgVpnName}/replicatedTopics/{replicatedTopic} | Delete a Replicated Topic object.
[**GetMsgVpnReplicatedTopic**](ReplicatedTopicApi.md#getmsgvpnreplicatedtopic) | **GET** /msgVpns/{msgVpnName}/replicatedTopics/{replicatedTopic} | Get a Replicated Topic object.
[**GetMsgVpnReplicatedTopics**](ReplicatedTopicApi.md#getmsgvpnreplicatedtopics) | **GET** /msgVpns/{msgVpnName}/replicatedTopics | Get a list of Replicated Topic objects.
[**ReplaceMsgVpnReplicatedTopic**](ReplicatedTopicApi.md#replacemsgvpnreplicatedtopic) | **PUT** /msgVpns/{msgVpnName}/replicatedTopics/{replicatedTopic} | Replace a Replicated Topic object.
[**UpdateMsgVpnReplicatedTopic**](ReplicatedTopicApi.md#updatemsgvpnreplicatedtopic) | **PATCH** /msgVpns/{msgVpnName}/replicatedTopics/{replicatedTopic} | Update a Replicated Topic object.

<a name="createmsgvpnreplicatedtopic"></a>
# **CreateMsgVpnReplicatedTopic**
> MsgVpnReplicatedTopicResponse CreateMsgVpnReplicatedTopic (MsgVpnReplicatedTopic body, string msgVpnName, string opaquePassword = null, List<string> select = null)

Create a Replicated Topic object.

Create a Replicated Topic object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  To indicate which messages should be replicated between the active and standby site, a Replicated Topic subscription must be configured on a Message VPN. If a published message matches both a replicated topic and an endpoint on the active site, then the message is replicated to the standby site.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| replicatedTopic|x|x||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.1.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnReplicatedTopicExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new ReplicatedTopicApi();
            var body = new MsgVpnReplicatedTopic(); // MsgVpnReplicatedTopic | The Replicated Topic object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Replicated Topic object.
                MsgVpnReplicatedTopicResponse result = apiInstance.CreateMsgVpnReplicatedTopic(body, msgVpnName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ReplicatedTopicApi.CreateMsgVpnReplicatedTopic: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnReplicatedTopic**](MsgVpnReplicatedTopic.md)| The Replicated Topic object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnReplicatedTopicResponse**](MsgVpnReplicatedTopicResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpnreplicatedtopic"></a>
# **DeleteMsgVpnReplicatedTopic**
> SempMetaOnlyResponse DeleteMsgVpnReplicatedTopic (string msgVpnName, string replicatedTopic)

Delete a Replicated Topic object.

Delete a Replicated Topic object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  To indicate which messages should be replicated between the active and standby site, a Replicated Topic subscription must be configured on a Message VPN. If a published message matches both a replicated topic and an endpoint on the active site, then the message is replicated to the standby site.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.1.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnReplicatedTopicExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new ReplicatedTopicApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var replicatedTopic = replicatedTopic_example;  // string | The topic for applying replication. Published messages matching this topic will be replicated to the standby site.

            try
            {
                // Delete a Replicated Topic object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnReplicatedTopic(msgVpnName, replicatedTopic);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ReplicatedTopicApi.DeleteMsgVpnReplicatedTopic: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **replicatedTopic** | **string**| The topic for applying replication. Published messages matching this topic will be replicated to the standby site. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnreplicatedtopic"></a>
# **GetMsgVpnReplicatedTopic**
> MsgVpnReplicatedTopicResponse GetMsgVpnReplicatedTopic (string msgVpnName, string replicatedTopic, string opaquePassword = null, List<string> select = null)

Get a Replicated Topic object.

Get a Replicated Topic object.  To indicate which messages should be replicated between the active and standby site, a Replicated Topic subscription must be configured on a Message VPN. If a published message matches both a replicated topic and an endpoint on the active site, then the message is replicated to the standby site.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| replicatedTopic|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.1.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnReplicatedTopicExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new ReplicatedTopicApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var replicatedTopic = replicatedTopic_example;  // string | The topic for applying replication. Published messages matching this topic will be replicated to the standby site.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Replicated Topic object.
                MsgVpnReplicatedTopicResponse result = apiInstance.GetMsgVpnReplicatedTopic(msgVpnName, replicatedTopic, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ReplicatedTopicApi.GetMsgVpnReplicatedTopic: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **replicatedTopic** | **string**| The topic for applying replication. Published messages matching this topic will be replicated to the standby site. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnReplicatedTopicResponse**](MsgVpnReplicatedTopicResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnreplicatedtopics"></a>
# **GetMsgVpnReplicatedTopics**
> MsgVpnReplicatedTopicsResponse GetMsgVpnReplicatedTopics (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Replicated Topic objects.

Get a list of Replicated Topic objects.  To indicate which messages should be replicated between the active and standby site, a Replicated Topic subscription must be configured on a Message VPN. If a published message matches both a replicated topic and an endpoint on the active site, then the message is replicated to the standby site.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| replicatedTopic|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.1.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnReplicatedTopicsExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new ReplicatedTopicApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Replicated Topic objects.
                MsgVpnReplicatedTopicsResponse result = apiInstance.GetMsgVpnReplicatedTopics(msgVpnName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ReplicatedTopicApi.GetMsgVpnReplicatedTopics: " + e.Message );
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

[**MsgVpnReplicatedTopicsResponse**](MsgVpnReplicatedTopicsResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacemsgvpnreplicatedtopic"></a>
# **ReplaceMsgVpnReplicatedTopic**
> MsgVpnReplicatedTopicResponse ReplaceMsgVpnReplicatedTopic (MsgVpnReplicatedTopic body, string msgVpnName, string replicatedTopic, string opaquePassword = null, List<string> select = null)

Replace a Replicated Topic object.

Replace a Replicated Topic object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  To indicate which messages should be replicated between the active and standby site, a Replicated Topic subscription must be configured on a Message VPN. If a published message matches both a replicated topic and an endpoint on the active site, then the message is replicated to the standby site.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| replicatedTopic|x||x|||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.1.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceMsgVpnReplicatedTopicExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new ReplicatedTopicApi();
            var body = new MsgVpnReplicatedTopic(); // MsgVpnReplicatedTopic | The Replicated Topic object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var replicatedTopic = replicatedTopic_example;  // string | The topic for applying replication. Published messages matching this topic will be replicated to the standby site.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Replicated Topic object.
                MsgVpnReplicatedTopicResponse result = apiInstance.ReplaceMsgVpnReplicatedTopic(body, msgVpnName, replicatedTopic, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ReplicatedTopicApi.ReplaceMsgVpnReplicatedTopic: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnReplicatedTopic**](MsgVpnReplicatedTopic.md)| The Replicated Topic object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **replicatedTopic** | **string**| The topic for applying replication. Published messages matching this topic will be replicated to the standby site. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnReplicatedTopicResponse**](MsgVpnReplicatedTopicResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatemsgvpnreplicatedtopic"></a>
# **UpdateMsgVpnReplicatedTopic**
> MsgVpnReplicatedTopicResponse UpdateMsgVpnReplicatedTopic (MsgVpnReplicatedTopic body, string msgVpnName, string replicatedTopic, string opaquePassword = null, List<string> select = null)

Update a Replicated Topic object.

Update a Replicated Topic object. Any attribute missing from the request will be left unchanged.  To indicate which messages should be replicated between the active and standby site, a Replicated Topic subscription must be configured on a Message VPN. If a published message matches both a replicated topic and an endpoint on the active site, then the message is replicated to the standby site.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| replicatedTopic|x|x|||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.1.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateMsgVpnReplicatedTopicExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new ReplicatedTopicApi();
            var body = new MsgVpnReplicatedTopic(); // MsgVpnReplicatedTopic | The Replicated Topic object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var replicatedTopic = replicatedTopic_example;  // string | The topic for applying replication. Published messages matching this topic will be replicated to the standby site.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Replicated Topic object.
                MsgVpnReplicatedTopicResponse result = apiInstance.UpdateMsgVpnReplicatedTopic(body, msgVpnName, replicatedTopic, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ReplicatedTopicApi.UpdateMsgVpnReplicatedTopic: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnReplicatedTopic**](MsgVpnReplicatedTopic.md)| The Replicated Topic object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **replicatedTopic** | **string**| The topic for applying replication. Published messages matching this topic will be replicated to the standby site. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnReplicatedTopicResponse**](MsgVpnReplicatedTopicResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
