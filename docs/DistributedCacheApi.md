# Semp.V2.CSharp.Api.DistributedCacheApi

All URIs are relative to *http://www.solace.com/SEMP/v2/config*

Method | HTTP request | Description
------------- | ------------- | -------------
[**CreateMsgVpnDistributedCache**](DistributedCacheApi.md#createmsgvpndistributedcache) | **POST** /msgVpns/{msgVpnName}/distributedCaches | Create a Distributed Cache object.
[**CreateMsgVpnDistributedCacheCluster**](DistributedCacheApi.md#createmsgvpndistributedcachecluster) | **POST** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters | Create a Cache Cluster object.
[**CreateMsgVpnDistributedCacheClusterGlobalCachingHomeCluster**](DistributedCacheApi.md#createmsgvpndistributedcacheclusterglobalcachinghomecluster) | **POST** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters | Create a Home Cache Cluster object.
[**CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix**](DistributedCacheApi.md#createmsgvpndistributedcacheclusterglobalcachinghomeclustertopicprefix) | **POST** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes | Create a Topic Prefix object.
[**CreateMsgVpnDistributedCacheClusterInstance**](DistributedCacheApi.md#createmsgvpndistributedcacheclusterinstance) | **POST** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances | Create a Cache Instance object.
[**CreateMsgVpnDistributedCacheClusterTopic**](DistributedCacheApi.md#createmsgvpndistributedcacheclustertopic) | **POST** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics | Create a Topic object.
[**DeleteMsgVpnDistributedCache**](DistributedCacheApi.md#deletemsgvpndistributedcache) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName} | Delete a Distributed Cache object.
[**DeleteMsgVpnDistributedCacheCluster**](DistributedCacheApi.md#deletemsgvpndistributedcachecluster) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName} | Delete a Cache Cluster object.
[**DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeCluster**](DistributedCacheApi.md#deletemsgvpndistributedcacheclusterglobalcachinghomecluster) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName} | Delete a Home Cache Cluster object.
[**DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix**](DistributedCacheApi.md#deletemsgvpndistributedcacheclusterglobalcachinghomeclustertopicprefix) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes/{topicPrefix} | Delete a Topic Prefix object.
[**DeleteMsgVpnDistributedCacheClusterInstance**](DistributedCacheApi.md#deletemsgvpndistributedcacheclusterinstance) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName} | Delete a Cache Instance object.
[**DeleteMsgVpnDistributedCacheClusterTopic**](DistributedCacheApi.md#deletemsgvpndistributedcacheclustertopic) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics/{topic} | Delete a Topic object.
[**GetMsgVpnDistributedCache**](DistributedCacheApi.md#getmsgvpndistributedcache) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName} | Get a Distributed Cache object.
[**GetMsgVpnDistributedCacheCluster**](DistributedCacheApi.md#getmsgvpndistributedcachecluster) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName} | Get a Cache Cluster object.
[**GetMsgVpnDistributedCacheClusterGlobalCachingHomeCluster**](DistributedCacheApi.md#getmsgvpndistributedcacheclusterglobalcachinghomecluster) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName} | Get a Home Cache Cluster object.
[**GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix**](DistributedCacheApi.md#getmsgvpndistributedcacheclusterglobalcachinghomeclustertopicprefix) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes/{topicPrefix} | Get a Topic Prefix object.
[**GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixes**](DistributedCacheApi.md#getmsgvpndistributedcacheclusterglobalcachinghomeclustertopicprefixes) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes | Get a list of Topic Prefix objects.
[**GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusters**](DistributedCacheApi.md#getmsgvpndistributedcacheclusterglobalcachinghomeclusters) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters | Get a list of Home Cache Cluster objects.
[**GetMsgVpnDistributedCacheClusterInstance**](DistributedCacheApi.md#getmsgvpndistributedcacheclusterinstance) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName} | Get a Cache Instance object.
[**GetMsgVpnDistributedCacheClusterInstances**](DistributedCacheApi.md#getmsgvpndistributedcacheclusterinstances) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances | Get a list of Cache Instance objects.
[**GetMsgVpnDistributedCacheClusterTopic**](DistributedCacheApi.md#getmsgvpndistributedcacheclustertopic) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics/{topic} | Get a Topic object.
[**GetMsgVpnDistributedCacheClusterTopics**](DistributedCacheApi.md#getmsgvpndistributedcacheclustertopics) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics | Get a list of Topic objects.
[**GetMsgVpnDistributedCacheClusters**](DistributedCacheApi.md#getmsgvpndistributedcacheclusters) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters | Get a list of Cache Cluster objects.
[**GetMsgVpnDistributedCaches**](DistributedCacheApi.md#getmsgvpndistributedcaches) | **GET** /msgVpns/{msgVpnName}/distributedCaches | Get a list of Distributed Cache objects.
[**ReplaceMsgVpnDistributedCache**](DistributedCacheApi.md#replacemsgvpndistributedcache) | **PUT** /msgVpns/{msgVpnName}/distributedCaches/{cacheName} | Replace a Distributed Cache object.
[**ReplaceMsgVpnDistributedCacheCluster**](DistributedCacheApi.md#replacemsgvpndistributedcachecluster) | **PUT** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName} | Replace a Cache Cluster object.
[**ReplaceMsgVpnDistributedCacheClusterInstance**](DistributedCacheApi.md#replacemsgvpndistributedcacheclusterinstance) | **PUT** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName} | Replace a Cache Instance object.
[**UpdateMsgVpnDistributedCache**](DistributedCacheApi.md#updatemsgvpndistributedcache) | **PATCH** /msgVpns/{msgVpnName}/distributedCaches/{cacheName} | Update a Distributed Cache object.
[**UpdateMsgVpnDistributedCacheCluster**](DistributedCacheApi.md#updatemsgvpndistributedcachecluster) | **PATCH** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName} | Update a Cache Cluster object.
[**UpdateMsgVpnDistributedCacheClusterInstance**](DistributedCacheApi.md#updatemsgvpndistributedcacheclusterinstance) | **PATCH** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName} | Update a Cache Instance object.

<a name="createmsgvpndistributedcache"></a>
# **CreateMsgVpnDistributedCache**
> MsgVpnDistributedCacheResponse CreateMsgVpnDistributedCache (MsgVpnDistributedCache body, string msgVpnName, string opaquePassword = null, List<string> select = null)

Create a Distributed Cache object.

Create a Distributed Cache object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x|x|||| msgVpnName|x||x|||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnDistributedCacheExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var body = new MsgVpnDistributedCache(); // MsgVpnDistributedCache | The Distributed Cache object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Distributed Cache object.
                MsgVpnDistributedCacheResponse result = apiInstance.CreateMsgVpnDistributedCache(body, msgVpnName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.CreateMsgVpnDistributedCache: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnDistributedCache**](MsgVpnDistributedCache.md)| The Distributed Cache object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheResponse**](MsgVpnDistributedCacheResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createmsgvpndistributedcachecluster"></a>
# **CreateMsgVpnDistributedCacheCluster**
> MsgVpnDistributedCacheClusterResponse CreateMsgVpnDistributedCacheCluster (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null)

Create a Cache Cluster object.

Create a Cache Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x|x|||| msgVpnName|x||x|||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnDistributedCacheClusterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var body = new MsgVpnDistributedCacheCluster(); // MsgVpnDistributedCacheCluster | The Cache Cluster object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Cache Cluster object.
                MsgVpnDistributedCacheClusterResponse result = apiInstance.CreateMsgVpnDistributedCacheCluster(body, msgVpnName, cacheName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.CreateMsgVpnDistributedCacheCluster: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnDistributedCacheCluster**](MsgVpnDistributedCacheCluster.md)| The Cache Cluster object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheClusterResponse**](MsgVpnDistributedCacheClusterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createmsgvpndistributedcacheclusterglobalcachinghomecluster"></a>
# **CreateMsgVpnDistributedCacheClusterGlobalCachingHomeCluster**
> MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse CreateMsgVpnDistributedCacheClusterGlobalCachingHomeCluster (MsgVpnDistributedCacheClusterGlobalCachingHomeCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)

Create a Home Cache Cluster object.

Create a Home Cache Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Home Cache Cluster is a Cache Cluster that is the \"definitive\" Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| homeClusterName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var body = new MsgVpnDistributedCacheClusterGlobalCachingHomeCluster(); // MsgVpnDistributedCacheClusterGlobalCachingHomeCluster | The Home Cache Cluster object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var clusterName = clusterName_example;  // string | The name of the Cache Cluster.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Home Cache Cluster object.
                MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse result = apiInstance.CreateMsgVpnDistributedCacheClusterGlobalCachingHomeCluster(body, msgVpnName, cacheName, clusterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.CreateMsgVpnDistributedCacheClusterGlobalCachingHomeCluster: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnDistributedCacheClusterGlobalCachingHomeCluster**](MsgVpnDistributedCacheClusterGlobalCachingHomeCluster.md)| The Home Cache Cluster object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **clusterName** | **string**| The name of the Cache Cluster. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse**](MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createmsgvpndistributedcacheclusterglobalcachinghomeclustertopicprefix"></a>
# **CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix**
> MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix body, string msgVpnName, string cacheName, string clusterName, string homeClusterName, string opaquePassword = null, List<string> select = null)

Create a Topic Prefix object.

Create a Topic Prefix object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| homeClusterName|x||x||| msgVpnName|x||x||| topicPrefix|x|x||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var body = new MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix(); // MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix | The Topic Prefix object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var clusterName = clusterName_example;  // string | The name of the Cache Cluster.
            var homeClusterName = homeClusterName_example;  // string | The name of the remote Home Cache Cluster.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Topic Prefix object.
                MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse result = apiInstance.CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix(body, msgVpnName, cacheName, clusterName, homeClusterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix**](MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix.md)| The Topic Prefix object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **clusterName** | **string**| The name of the Cache Cluster. | 
 **homeClusterName** | **string**| The name of the remote Home Cache Cluster. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse**](MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createmsgvpndistributedcacheclusterinstance"></a>
# **CreateMsgVpnDistributedCacheClusterInstance**
> MsgVpnDistributedCacheClusterInstanceResponse CreateMsgVpnDistributedCacheClusterInstance (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)

Create a Cache Instance object.

Create a Cache Instance object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| instanceName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnDistributedCacheClusterInstanceExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var body = new MsgVpnDistributedCacheClusterInstance(); // MsgVpnDistributedCacheClusterInstance | The Cache Instance object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var clusterName = clusterName_example;  // string | The name of the Cache Cluster.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Cache Instance object.
                MsgVpnDistributedCacheClusterInstanceResponse result = apiInstance.CreateMsgVpnDistributedCacheClusterInstance(body, msgVpnName, cacheName, clusterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.CreateMsgVpnDistributedCacheClusterInstance: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnDistributedCacheClusterInstance**](MsgVpnDistributedCacheClusterInstance.md)| The Cache Instance object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **clusterName** | **string**| The name of the Cache Cluster. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheClusterInstanceResponse**](MsgVpnDistributedCacheClusterInstanceResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createmsgvpndistributedcacheclustertopic"></a>
# **CreateMsgVpnDistributedCacheClusterTopic**
> MsgVpnDistributedCacheClusterTopicResponse CreateMsgVpnDistributedCacheClusterTopic (MsgVpnDistributedCacheClusterTopic body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)

Create a Topic object.

Create a Topic object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| msgVpnName|x||x||| topic|x|x||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnDistributedCacheClusterTopicExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var body = new MsgVpnDistributedCacheClusterTopic(); // MsgVpnDistributedCacheClusterTopic | The Topic object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var clusterName = clusterName_example;  // string | The name of the Cache Cluster.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Topic object.
                MsgVpnDistributedCacheClusterTopicResponse result = apiInstance.CreateMsgVpnDistributedCacheClusterTopic(body, msgVpnName, cacheName, clusterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.CreateMsgVpnDistributedCacheClusterTopic: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnDistributedCacheClusterTopic**](MsgVpnDistributedCacheClusterTopic.md)| The Topic object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **clusterName** | **string**| The name of the Cache Cluster. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheClusterTopicResponse**](MsgVpnDistributedCacheClusterTopicResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpndistributedcache"></a>
# **DeleteMsgVpnDistributedCache**
> SempMetaOnlyResponse DeleteMsgVpnDistributedCache (string msgVpnName, string cacheName)

Delete a Distributed Cache object.

Delete a Distributed Cache object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnDistributedCacheExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.

            try
            {
                // Delete a Distributed Cache object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnDistributedCache(msgVpnName, cacheName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.DeleteMsgVpnDistributedCache: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpndistributedcachecluster"></a>
# **DeleteMsgVpnDistributedCacheCluster**
> SempMetaOnlyResponse DeleteMsgVpnDistributedCacheCluster (string msgVpnName, string cacheName, string clusterName)

Delete a Cache Cluster object.

Delete a Cache Cluster object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnDistributedCacheClusterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var clusterName = clusterName_example;  // string | The name of the Cache Cluster.

            try
            {
                // Delete a Cache Cluster object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnDistributedCacheCluster(msgVpnName, cacheName, clusterName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.DeleteMsgVpnDistributedCacheCluster: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **clusterName** | **string**| The name of the Cache Cluster. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpndistributedcacheclusterglobalcachinghomecluster"></a>
# **DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeCluster**
> SempMetaOnlyResponse DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeCluster (string msgVpnName, string cacheName, string clusterName, string homeClusterName)

Delete a Home Cache Cluster object.

Delete a Home Cache Cluster object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Home Cache Cluster is a Cache Cluster that is the \"definitive\" Cache Cluster for a given topic in the context of the Global Caching feature.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var clusterName = clusterName_example;  // string | The name of the Cache Cluster.
            var homeClusterName = homeClusterName_example;  // string | The name of the remote Home Cache Cluster.

            try
            {
                // Delete a Home Cache Cluster object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeCluster(msgVpnName, cacheName, clusterName, homeClusterName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeCluster: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **clusterName** | **string**| The name of the Cache Cluster. | 
 **homeClusterName** | **string**| The name of the remote Home Cache Cluster. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpndistributedcacheclusterglobalcachinghomeclustertopicprefix"></a>
# **DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix**
> SempMetaOnlyResponse DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string topicPrefix)

Delete a Topic Prefix object.

Delete a Topic Prefix object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var clusterName = clusterName_example;  // string | The name of the Cache Cluster.
            var homeClusterName = homeClusterName_example;  // string | The name of the remote Home Cache Cluster.
            var topicPrefix = topicPrefix_example;  // string | A topic prefix for global topics available from the remote Home Cache Cluster. A wildcard (/>) is implied at the end of the prefix.

            try
            {
                // Delete a Topic Prefix object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix(msgVpnName, cacheName, clusterName, homeClusterName, topicPrefix);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **clusterName** | **string**| The name of the Cache Cluster. | 
 **homeClusterName** | **string**| The name of the remote Home Cache Cluster. | 
 **topicPrefix** | **string**| A topic prefix for global topics available from the remote Home Cache Cluster. A wildcard (/&gt;) is implied at the end of the prefix. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpndistributedcacheclusterinstance"></a>
# **DeleteMsgVpnDistributedCacheClusterInstance**
> SempMetaOnlyResponse DeleteMsgVpnDistributedCacheClusterInstance (string msgVpnName, string cacheName, string clusterName, string instanceName)

Delete a Cache Instance object.

Delete a Cache Instance object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnDistributedCacheClusterInstanceExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var clusterName = clusterName_example;  // string | The name of the Cache Cluster.
            var instanceName = instanceName_example;  // string | The name of the Cache Instance.

            try
            {
                // Delete a Cache Instance object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnDistributedCacheClusterInstance(msgVpnName, cacheName, clusterName, instanceName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.DeleteMsgVpnDistributedCacheClusterInstance: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **clusterName** | **string**| The name of the Cache Cluster. | 
 **instanceName** | **string**| The name of the Cache Instance. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpndistributedcacheclustertopic"></a>
# **DeleteMsgVpnDistributedCacheClusterTopic**
> SempMetaOnlyResponse DeleteMsgVpnDistributedCacheClusterTopic (string msgVpnName, string cacheName, string clusterName, string topic)

Delete a Topic object.

Delete a Topic object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnDistributedCacheClusterTopicExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var clusterName = clusterName_example;  // string | The name of the Cache Cluster.
            var topic = topic_example;  // string | The value of the Topic in the form a/b/c.

            try
            {
                // Delete a Topic object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnDistributedCacheClusterTopic(msgVpnName, cacheName, clusterName, topic);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.DeleteMsgVpnDistributedCacheClusterTopic: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **clusterName** | **string**| The name of the Cache Cluster. | 
 **topic** | **string**| The value of the Topic in the form a/b/c. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpndistributedcache"></a>
# **GetMsgVpnDistributedCache**
> MsgVpnDistributedCacheResponse GetMsgVpnDistributedCache (string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null)

Get a Distributed Cache object.

Get a Distributed Cache object.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnDistributedCacheExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Distributed Cache object.
                MsgVpnDistributedCacheResponse result = apiInstance.GetMsgVpnDistributedCache(msgVpnName, cacheName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.GetMsgVpnDistributedCache: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheResponse**](MsgVpnDistributedCacheResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpndistributedcachecluster"></a>
# **GetMsgVpnDistributedCacheCluster**
> MsgVpnDistributedCacheClusterResponse GetMsgVpnDistributedCacheCluster (string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)

Get a Cache Cluster object.

Get a Cache Cluster object.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnDistributedCacheClusterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var clusterName = clusterName_example;  // string | The name of the Cache Cluster.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Cache Cluster object.
                MsgVpnDistributedCacheClusterResponse result = apiInstance.GetMsgVpnDistributedCacheCluster(msgVpnName, cacheName, clusterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.GetMsgVpnDistributedCacheCluster: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **clusterName** | **string**| The name of the Cache Cluster. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheClusterResponse**](MsgVpnDistributedCacheClusterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpndistributedcacheclusterglobalcachinghomecluster"></a>
# **GetMsgVpnDistributedCacheClusterGlobalCachingHomeCluster**
> MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse GetMsgVpnDistributedCacheClusterGlobalCachingHomeCluster (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string opaquePassword = null, List<string> select = null)

Get a Home Cache Cluster object.

Get a Home Cache Cluster object.  A Home Cache Cluster is a Cache Cluster that is the \"definitive\" Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var clusterName = clusterName_example;  // string | The name of the Cache Cluster.
            var homeClusterName = homeClusterName_example;  // string | The name of the remote Home Cache Cluster.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Home Cache Cluster object.
                MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse result = apiInstance.GetMsgVpnDistributedCacheClusterGlobalCachingHomeCluster(msgVpnName, cacheName, clusterName, homeClusterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.GetMsgVpnDistributedCacheClusterGlobalCachingHomeCluster: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **clusterName** | **string**| The name of the Cache Cluster. | 
 **homeClusterName** | **string**| The name of the remote Home Cache Cluster. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse**](MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpndistributedcacheclusterglobalcachinghomeclustertopicprefix"></a>
# **GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix**
> MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string topicPrefix, string opaquePassword = null, List<string> select = null)

Get a Topic Prefix object.

Get a Topic Prefix object.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x||| topicPrefix|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var clusterName = clusterName_example;  // string | The name of the Cache Cluster.
            var homeClusterName = homeClusterName_example;  // string | The name of the remote Home Cache Cluster.
            var topicPrefix = topicPrefix_example;  // string | A topic prefix for global topics available from the remote Home Cache Cluster. A wildcard (/>) is implied at the end of the prefix.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Topic Prefix object.
                MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse result = apiInstance.GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix(msgVpnName, cacheName, clusterName, homeClusterName, topicPrefix, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **clusterName** | **string**| The name of the Cache Cluster. | 
 **homeClusterName** | **string**| The name of the remote Home Cache Cluster. | 
 **topicPrefix** | **string**| A topic prefix for global topics available from the remote Home Cache Cluster. A wildcard (/&gt;) is implied at the end of the prefix. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse**](MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpndistributedcacheclusterglobalcachinghomeclustertopicprefixes"></a>
# **GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixes**
> MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixes (string msgVpnName, string cacheName, string clusterName, string homeClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Topic Prefix objects.

Get a list of Topic Prefix objects.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x||| topicPrefix|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var clusterName = clusterName_example;  // string | The name of the Cache Cluster.
            var homeClusterName = homeClusterName_example;  // string | The name of the remote Home Cache Cluster.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Topic Prefix objects.
                MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse result = apiInstance.GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixes(msgVpnName, cacheName, clusterName, homeClusterName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixes: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **clusterName** | **string**| The name of the Cache Cluster. | 
 **homeClusterName** | **string**| The name of the remote Home Cache Cluster. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse**](MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpndistributedcacheclusterglobalcachinghomeclusters"></a>
# **GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusters**
> MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusters (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Home Cache Cluster objects.

Get a list of Home Cache Cluster objects.  A Home Cache Cluster is a Cache Cluster that is the \"definitive\" Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnDistributedCacheClusterGlobalCachingHomeClustersExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var clusterName = clusterName_example;  // string | The name of the Cache Cluster.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Home Cache Cluster objects.
                MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse result = apiInstance.GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusters(msgVpnName, cacheName, clusterName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusters: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **clusterName** | **string**| The name of the Cache Cluster. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse**](MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpndistributedcacheclusterinstance"></a>
# **GetMsgVpnDistributedCacheClusterInstance**
> MsgVpnDistributedCacheClusterInstanceResponse GetMsgVpnDistributedCacheClusterInstance (string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null)

Get a Cache Instance object.

Get a Cache Instance object.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| instanceName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnDistributedCacheClusterInstanceExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var clusterName = clusterName_example;  // string | The name of the Cache Cluster.
            var instanceName = instanceName_example;  // string | The name of the Cache Instance.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Cache Instance object.
                MsgVpnDistributedCacheClusterInstanceResponse result = apiInstance.GetMsgVpnDistributedCacheClusterInstance(msgVpnName, cacheName, clusterName, instanceName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.GetMsgVpnDistributedCacheClusterInstance: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **clusterName** | **string**| The name of the Cache Cluster. | 
 **instanceName** | **string**| The name of the Cache Instance. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheClusterInstanceResponse**](MsgVpnDistributedCacheClusterInstanceResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpndistributedcacheclusterinstances"></a>
# **GetMsgVpnDistributedCacheClusterInstances**
> MsgVpnDistributedCacheClusterInstancesResponse GetMsgVpnDistributedCacheClusterInstances (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Cache Instance objects.

Get a list of Cache Instance objects.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| instanceName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnDistributedCacheClusterInstancesExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var clusterName = clusterName_example;  // string | The name of the Cache Cluster.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Cache Instance objects.
                MsgVpnDistributedCacheClusterInstancesResponse result = apiInstance.GetMsgVpnDistributedCacheClusterInstances(msgVpnName, cacheName, clusterName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.GetMsgVpnDistributedCacheClusterInstances: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **clusterName** | **string**| The name of the Cache Cluster. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheClusterInstancesResponse**](MsgVpnDistributedCacheClusterInstancesResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpndistributedcacheclustertopic"></a>
# **GetMsgVpnDistributedCacheClusterTopic**
> MsgVpnDistributedCacheClusterTopicResponse GetMsgVpnDistributedCacheClusterTopic (string msgVpnName, string cacheName, string clusterName, string topic, string opaquePassword = null, List<string> select = null)

Get a Topic object.

Get a Topic object.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x||| topic|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnDistributedCacheClusterTopicExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var clusterName = clusterName_example;  // string | The name of the Cache Cluster.
            var topic = topic_example;  // string | The value of the Topic in the form a/b/c.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Topic object.
                MsgVpnDistributedCacheClusterTopicResponse result = apiInstance.GetMsgVpnDistributedCacheClusterTopic(msgVpnName, cacheName, clusterName, topic, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.GetMsgVpnDistributedCacheClusterTopic: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **clusterName** | **string**| The name of the Cache Cluster. | 
 **topic** | **string**| The value of the Topic in the form a/b/c. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheClusterTopicResponse**](MsgVpnDistributedCacheClusterTopicResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpndistributedcacheclustertopics"></a>
# **GetMsgVpnDistributedCacheClusterTopics**
> MsgVpnDistributedCacheClusterTopicsResponse GetMsgVpnDistributedCacheClusterTopics (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Topic objects.

Get a list of Topic objects.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x||| topic|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnDistributedCacheClusterTopicsExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var clusterName = clusterName_example;  // string | The name of the Cache Cluster.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Topic objects.
                MsgVpnDistributedCacheClusterTopicsResponse result = apiInstance.GetMsgVpnDistributedCacheClusterTopics(msgVpnName, cacheName, clusterName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.GetMsgVpnDistributedCacheClusterTopics: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **clusterName** | **string**| The name of the Cache Cluster. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheClusterTopicsResponse**](MsgVpnDistributedCacheClusterTopicsResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpndistributedcacheclusters"></a>
# **GetMsgVpnDistributedCacheClusters**
> MsgVpnDistributedCacheClustersResponse GetMsgVpnDistributedCacheClusters (string msgVpnName, string cacheName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Cache Cluster objects.

Get a list of Cache Cluster objects.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnDistributedCacheClustersExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Cache Cluster objects.
                MsgVpnDistributedCacheClustersResponse result = apiInstance.GetMsgVpnDistributedCacheClusters(msgVpnName, cacheName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.GetMsgVpnDistributedCacheClusters: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheClustersResponse**](MsgVpnDistributedCacheClustersResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpndistributedcaches"></a>
# **GetMsgVpnDistributedCaches**
> MsgVpnDistributedCachesResponse GetMsgVpnDistributedCaches (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Distributed Cache objects.

Get a list of Distributed Cache objects.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnDistributedCachesExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Distributed Cache objects.
                MsgVpnDistributedCachesResponse result = apiInstance.GetMsgVpnDistributedCaches(msgVpnName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.GetMsgVpnDistributedCaches: " + e.Message );
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

[**MsgVpnDistributedCachesResponse**](MsgVpnDistributedCachesResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacemsgvpndistributedcache"></a>
# **ReplaceMsgVpnDistributedCache**
> MsgVpnDistributedCacheResponse ReplaceMsgVpnDistributedCache (MsgVpnDistributedCache body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null)

Replace a Distributed Cache object.

Replace a Distributed Cache object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| cacheVirtualRouter||x|||||| msgVpnName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceMsgVpnDistributedCacheExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var body = new MsgVpnDistributedCache(); // MsgVpnDistributedCache | The Distributed Cache object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Distributed Cache object.
                MsgVpnDistributedCacheResponse result = apiInstance.ReplaceMsgVpnDistributedCache(body, msgVpnName, cacheName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.ReplaceMsgVpnDistributedCache: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnDistributedCache**](MsgVpnDistributedCache.md)| The Distributed Cache object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheResponse**](MsgVpnDistributedCacheResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacemsgvpndistributedcachecluster"></a>
# **ReplaceMsgVpnDistributedCacheCluster**
> MsgVpnDistributedCacheClusterResponse ReplaceMsgVpnDistributedCacheCluster (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)

Replace a Cache Cluster object.

Replace a Cache Cluster object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| clusterName|x||x||||| deliverToOneOverrideEnabled||||||x|| msgVpnName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceMsgVpnDistributedCacheClusterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var body = new MsgVpnDistributedCacheCluster(); // MsgVpnDistributedCacheCluster | The Cache Cluster object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var clusterName = clusterName_example;  // string | The name of the Cache Cluster.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Cache Cluster object.
                MsgVpnDistributedCacheClusterResponse result = apiInstance.ReplaceMsgVpnDistributedCacheCluster(body, msgVpnName, cacheName, clusterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.ReplaceMsgVpnDistributedCacheCluster: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnDistributedCacheCluster**](MsgVpnDistributedCacheCluster.md)| The Cache Cluster object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **clusterName** | **string**| The name of the Cache Cluster. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheClusterResponse**](MsgVpnDistributedCacheClusterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacemsgvpndistributedcacheclusterinstance"></a>
# **ReplaceMsgVpnDistributedCacheClusterInstance**
> MsgVpnDistributedCacheClusterInstanceResponse ReplaceMsgVpnDistributedCacheClusterInstance (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null)

Replace a Cache Instance object.

Replace a Cache Instance object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| clusterName|x||x||||| instanceName|x||x||||| msgVpnName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceMsgVpnDistributedCacheClusterInstanceExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var body = new MsgVpnDistributedCacheClusterInstance(); // MsgVpnDistributedCacheClusterInstance | The Cache Instance object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var clusterName = clusterName_example;  // string | The name of the Cache Cluster.
            var instanceName = instanceName_example;  // string | The name of the Cache Instance.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Cache Instance object.
                MsgVpnDistributedCacheClusterInstanceResponse result = apiInstance.ReplaceMsgVpnDistributedCacheClusterInstance(body, msgVpnName, cacheName, clusterName, instanceName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.ReplaceMsgVpnDistributedCacheClusterInstance: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnDistributedCacheClusterInstance**](MsgVpnDistributedCacheClusterInstance.md)| The Cache Instance object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **clusterName** | **string**| The name of the Cache Cluster. | 
 **instanceName** | **string**| The name of the Cache Instance. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheClusterInstanceResponse**](MsgVpnDistributedCacheClusterInstanceResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatemsgvpndistributedcache"></a>
# **UpdateMsgVpnDistributedCache**
> MsgVpnDistributedCacheResponse UpdateMsgVpnDistributedCache (MsgVpnDistributedCache body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null)

Update a Distributed Cache object.

Update a Distributed Cache object. Any attribute missing from the request will be left unchanged.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| cacheVirtualRouter||x||||| msgVpnName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateMsgVpnDistributedCacheExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var body = new MsgVpnDistributedCache(); // MsgVpnDistributedCache | The Distributed Cache object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Distributed Cache object.
                MsgVpnDistributedCacheResponse result = apiInstance.UpdateMsgVpnDistributedCache(body, msgVpnName, cacheName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.UpdateMsgVpnDistributedCache: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnDistributedCache**](MsgVpnDistributedCache.md)| The Distributed Cache object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheResponse**](MsgVpnDistributedCacheResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatemsgvpndistributedcachecluster"></a>
# **UpdateMsgVpnDistributedCacheCluster**
> MsgVpnDistributedCacheClusterResponse UpdateMsgVpnDistributedCacheCluster (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)

Update a Cache Cluster object.

Update a Cache Cluster object. Any attribute missing from the request will be left unchanged.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| clusterName|x|x||||| deliverToOneOverrideEnabled|||||x|| msgVpnName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateMsgVpnDistributedCacheClusterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var body = new MsgVpnDistributedCacheCluster(); // MsgVpnDistributedCacheCluster | The Cache Cluster object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var clusterName = clusterName_example;  // string | The name of the Cache Cluster.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Cache Cluster object.
                MsgVpnDistributedCacheClusterResponse result = apiInstance.UpdateMsgVpnDistributedCacheCluster(body, msgVpnName, cacheName, clusterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.UpdateMsgVpnDistributedCacheCluster: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnDistributedCacheCluster**](MsgVpnDistributedCacheCluster.md)| The Cache Cluster object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **clusterName** | **string**| The name of the Cache Cluster. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheClusterResponse**](MsgVpnDistributedCacheClusterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatemsgvpndistributedcacheclusterinstance"></a>
# **UpdateMsgVpnDistributedCacheClusterInstance**
> MsgVpnDistributedCacheClusterInstanceResponse UpdateMsgVpnDistributedCacheClusterInstance (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null)

Update a Cache Instance object.

Update a Cache Instance object. Any attribute missing from the request will be left unchanged.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| clusterName|x|x||||| instanceName|x|x||||| msgVpnName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.11.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateMsgVpnDistributedCacheClusterInstanceExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new DistributedCacheApi();
            var body = new MsgVpnDistributedCacheClusterInstance(); // MsgVpnDistributedCacheClusterInstance | The Cache Instance object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var cacheName = cacheName_example;  // string | The name of the Distributed Cache.
            var clusterName = clusterName_example;  // string | The name of the Cache Cluster.
            var instanceName = instanceName_example;  // string | The name of the Cache Instance.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Cache Instance object.
                MsgVpnDistributedCacheClusterInstanceResponse result = apiInstance.UpdateMsgVpnDistributedCacheClusterInstance(body, msgVpnName, cacheName, clusterName, instanceName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling DistributedCacheApi.UpdateMsgVpnDistributedCacheClusterInstance: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnDistributedCacheClusterInstance**](MsgVpnDistributedCacheClusterInstance.md)| The Cache Instance object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **cacheName** | **string**| The name of the Distributed Cache. | 
 **clusterName** | **string**| The name of the Cache Cluster. | 
 **instanceName** | **string**| The name of the Cache Instance. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnDistributedCacheClusterInstanceResponse**](MsgVpnDistributedCacheClusterInstanceResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
