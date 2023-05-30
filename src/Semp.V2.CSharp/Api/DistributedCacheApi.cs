/* 
 * SEMP (Solace Element Management Protocol)
 *
 * SEMP (starting in `v2`, see note 1) is a RESTful API for configuring, monitoring, and administering a Solace PubSub+ broker.  SEMP uses URIs to address manageable **resources** of the Solace PubSub+ broker. Resources are individual **objects**, **collections** of objects, or (exclusively in the action API) **actions**. This document applies to the following API:   API|Base Path|Purpose|Comments :- --|:- --|:- --|:- -- Configuration|/SEMP/v2/config|Reading and writing config state|See note 2    The following APIs are also available:   API|Base Path|Purpose|Comments :- --|:- --|:- --|:- -- Action|/SEMP/v2/action|Performing actions|See note 2 Monitoring|/SEMP/v2/monitor|Querying operational parameters|See note 2    Resources are always nouns, with individual objects being singular and collections being plural.  Objects within a collection are identified by an `obj-id`, which follows the collection name with the form `collection-name/obj-id`.  Actions within an object are identified by an `action-id`, which follows the object name with the form `obj-id/action-id`.  Some examples:  ``` /SEMP/v2/config/msgVpns                        ; MsgVpn collection /SEMP/v2/config/msgVpns/a                      ; MsgVpn object named \"a\" /SEMP/v2/config/msgVpns/a/queues               ; Queue collection in MsgVpn \"a\" /SEMP/v2/config/msgVpns/a/queues/b             ; Queue object named \"b\" in MsgVpn \"a\" /SEMP/v2/action/msgVpns/a/queues/b/startReplay ; Action that starts a replay on Queue \"b\" in MsgVpn \"a\" /SEMP/v2/monitor/msgVpns/a/clients             ; Client collection in MsgVpn \"a\" /SEMP/v2/monitor/msgVpns/a/clients/c           ; Client object named \"c\" in MsgVpn \"a\" ```  ## Collection Resources  Collections are unordered lists of objects (unless described as otherwise), and are described by JSON arrays. Each item in the array represents an object in the same manner as the individual object would normally be represented. In the configuration API, the creation of a new object is done through its collection resource.  ## Object and Action Resources  Objects are composed of attributes, actions, collections, and other objects. They are described by JSON objects as name/value pairs. The collections and actions of an object are not contained directly in the object's JSON content; rather the content includes an attribute containing a URI which points to the collections and actions. These contained resources must be managed through this URI. At a minimum, every object has one or more identifying attributes, and its own `uri` attribute which contains the URI pointing to itself.  Actions are also composed of attributes, and are described by JSON objects as name/value pairs. Unlike objects, however, they are not members of a collection and cannot be retrieved, only performed. Actions only exist in the action API.  Attributes in an object or action may have any combination of the following properties:   Property|Meaning|Comments :- --|:- --|:- -- Identifying|Attribute is involved in unique identification of the object, and appears in its URI| Const|Attribute value can only be chosen during object creation| Required|Attribute must be provided in the request| Read-Only|Attribute can only be read, not written.|See note 3 Write-Only|Attribute can only be written, not read, unless the attribute is also opaque|See the documentation for the opaque property Requires-Disable|Attribute cannot be changed while the object (or the relevant part of the object) is administratively enabled| Auto-Disable|Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as one or more attributes will be temporarily disabled to apply the change| Deprecated|Attribute is deprecated, and will disappear in the next SEMP version| Opaque|Attribute can be set or retrieved in opaque form when the `opaquePassword` query parameter is present|See the `opaquePassword` query parameter documentation    In some requests, certain attributes may only be provided in certain combinations with other attributes:   Relationship|Meaning :- --|:- -- Requires|Attribute may only be changed by a request if a particular attribute or combination of attributes is also provided in the request Conflicts|Attribute may only be provided in a request if a particular attribute or combination of attributes is not also provided in the request    In the monitoring API, any non-identifying attribute may not be returned in a GET.  ## HTTP Methods  The following HTTP methods manipulate resources in accordance with these general principles. Note that some methods are only used in certain APIs:   Method|Resource|Meaning|Request Body|Response Body|Notes :- --|:- --|:- --|:- --|:- --|:- -- POST|Collection|Create object|Initial attribute values|Object attributes and metadata|Absent attributes are set to default. If object already exists, a 400 error is returned PUT|Object|Update object|New attribute values|Object attributes and metadata|If does not exist, the object is first created. Absent attributes are set to default, with certain exceptions (see note 4) PUT|Action|Performs action|Action arguments|Action metadata| PATCH|Object|Update object|New attribute values|Object attributes and metadata|Absent attributes are left unchanged. If the object does not exist, a 404 error is returned DELETE|Object|Delete object|Empty|Object metadata|If the object does not exist, a 404 is returned GET|Object|Get object|Empty|Object attributes and metadata|If the object does not exist, a 404 is returned GET|Collection|Get collection|Empty|Object attributes and collection metadata|If the collection is empty, then an empty collection is returned with a 200 code    ## Common Query Parameters  The following are some common query parameters that are supported by many method/URI combinations. Individual URIs may document additional parameters. Note that multiple query parameters can be used together in a single URI, separated by the ampersand character. For example:  ``` ; Request for the MsgVpns collection using two hypothetical query parameters ; \"q1\" and \"q2\" with values \"val1\" and \"val2\" respectively /SEMP/v2/config/msgVpns?q1=val1&q2=val2 ```  ### select  Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. Use this query parameter to limit the size of the returned data for each returned object, return only those fields that are desired, or exclude fields that are not desired.  The value of `select` is a comma-separated list of attribute names. If the list contains attribute names that are not prefaced by `-`, only those attributes are included in the response. If the list contains attribute names that are prefaced by `-`, those attributes are excluded from the response. If the list contains both types, then the difference of the first set of attributes and the second set of attributes is returned. If the list is empty (i.e. `select=`), it is treated the same as if no `select` was provided: all attribute are returned.  All attributes that are prefaced by `-` must follow all attributes that are not prefaced by `-`. In addition, each attribute name in the list must match at least one attribute in the object.  Names may include the `*` wildcard (zero or more characters). Nested attribute names are supported using periods (e.g. `parentName.childName`).  Some examples:  ``` ; List of all MsgVpn names /SEMP/v2/config/msgVpns?select=msgVpnName ; List of all MsgVpn and their attributes except for their names /SEMP/v2/config/msgVpns?select=-msgVpnName ; Authentication attributes of MsgVpn \"finance\" /SEMP/v2/config/msgVpns/finance?select=authentication%2A ; All attributes of MsgVpn \"finance\" except for authentication attributes /SEMP/v2/config/msgVpns/finance?select=-authentication%2A ; Access related attributes of Queue \"orderQ\" of MsgVpn \"finance\" /SEMP/v2/config/msgVpns/finance/queues/orderQ?select=owner,permission ```  ### where  Include in the response only objects where certain conditions are true. Use this query parameter to limit which objects are returned to those whose attribute values meet the given conditions.  The value of `where` is a comma-separated list of expressions. All expressions must be true for the object to be included in the response. Each expression takes the form:  ``` expression  = attribute-name OP value OP          = '==' | '!=' | '<' | '>' | '<=' | '>=' ```  `value` may be a number, string, `true`, or `false`, as appropriate for the type of `attribute-name`. Greater-than and less-than comparisons only work for numbers. A `*` in a string `value` is interpreted as a wildcard (zero or more characters). Some examples:  ``` ; Only enabled MsgVpns /SEMP/v2/config/msgVpns?where=enabled%3D%3Dtrue ; Only MsgVpns using basic non-LDAP authentication /SEMP/v2/config/msgVpns?where=authenticationBasicEnabled%3D%3Dtrue,authenticationBasicType%21%3Dldap ; Only MsgVpns that allow more than 100 client connections /SEMP/v2/config/msgVpns?where=maxConnectionCount%3E100 ; Only MsgVpns with msgVpnName starting with \"B\": /SEMP/v2/config/msgVpns?where=msgVpnName%3D%3DB%2A ```  ### count  Limit the count of objects in the response. This can be useful to limit the size of the response for large collections. The minimum value for `count` is `1` and the default is `10`. There is also a per-collection maximum value to limit request handling time.  `count` does not guarantee that a minimum number of objects will be returned. A page may contain fewer than `count` objects or even be empty. Additional objects may nonetheless be available for retrieval on subsequent pages. See the `cursor` query parameter documentation for more information on paging.  For example: ``` ; Up to 25 MsgVpns /SEMP/v2/config/msgVpns?count=25 ```  ### cursor  The cursor, or position, for the next page of objects. Cursors are opaque data that should not be created or interpreted by SEMP clients, and should only be used as described below.  When a request is made for a collection and there may be additional objects available for retrieval that are not included in the initial response, the response will include a `cursorQuery` field containing a cursor. The value of this field can be specified in the `cursor` query parameter of a subsequent request to retrieve the next page of objects.  Applications must continue to use the `cursorQuery` if one is provided in order to retrieve the full set of objects associated with the request, even if a page contains fewer than the requested number of objects (see the `count` query parameter documentation) or is empty.  ### opaquePassword  Attributes with the opaque property are also write-only and so cannot normally be retrieved in a GET. However, when a password is provided in the `opaquePassword` query parameter, attributes with the opaque property are retrieved in a GET in opaque form, encrypted with this password. The query parameter can also be used on a POST, PATCH, or PUT to set opaque attributes using opaque attribute values retrieved in a GET, so long as:  1. the same password that was used to retrieve the opaque attribute values is provided; and  2. the broker to which the request is being sent has the same major and minor SEMP version as the broker that produced the opaque attribute values.  The password provided in the query parameter must be a minimum of 8 characters and a maximum of 128 characters.  The query parameter can only be used in the configuration API, and only over HTTPS.  ## Authentication  When a client makes its first SEMPv2 request, it must supply a username and password using HTTP Basic authentication, or an OAuth token or tokens using HTTP Bearer authentication.  When HTTP Basic authentication is used, the broker returns a cookie containing a session key. The client can omit the username and password from subsequent requests, because the broker can use the session cookie for authentication instead. When the session expires or is deleted, the client must provide the username and password again, and the broker creates a new session.  There are a limited number of session slots available on the broker. The broker returns 529 No SEMP Session Available if it is not able to allocate a session.  If certain attributes—such as a user's password—are changed, the broker automatically deletes the affected sessions. These attributes are documented below. However, changes in external user configuration data stored on a RADIUS or LDAP server do not trigger the broker to delete the associated session(s), therefore you must do this manually, if required.  A client can retrieve its current session information using the /about/user endpoint and delete its own session using the /about/user/logout endpoint. A client with appropriate permissions can also manage all sessions using the /sessions endpoint.  Sessions are not created when authenticating with an OAuth token or tokens using HTTP Bearer authentication. If a session cookie is provided, it is ignored.  ## Help  Visit [our website](https://solace.com) to learn more about Solace.  You can also download the SEMP API specifications by clicking [here](https://solace.com/downloads/).  If you need additional support, please contact us at [support@solace.com](mailto:support@solace.com).  ## Notes  Note|Description :- --:|:- -- 1|This specification defines SEMP starting in \"v2\", and not the original SEMP \"v1\" interface. Request and response formats between \"v1\" and \"v2\" are entirely incompatible, although both protocols share a common port configuration on the Solace PubSub+ broker. They are differentiated by the initial portion of the URI path, one of either \"/SEMP/\" or \"/SEMP/v2/\" 2|This API is partially implemented. Only a subset of all objects are available. 3|Read-only attributes may appear in POST and PUT/PATCH requests. However, if a read-only attribute is not marked as identifying, it will be ignored during a PUT/PATCH. 4|On a PUT, if the SEMP user is not authorized to modify the attribute, its value is left unchanged rather than set to default. In addition, the values of write-only attributes are not set to their defaults on a PUT, except in the following two cases: there is a mutual requires relationship with another non-write-only attribute, both attributes are absent from the request, and the non-write-only attribute is not currently set to its default value; or the attribute is also opaque and the `opaquePassword` query parameter is provided in the request.  
 *
 * OpenAPI spec version: 2.35
 * Contact: support@solace.com
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RestSharp.Portable;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Semp.V2.CSharp.Api
{
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
        public interface IDistributedCacheApi : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// Create a Distributed Cache object.
        /// </summary>
        /// <remarks>
        /// Create a Distributed Cache object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x|x|||| msgVpnName|x||x|||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheResponse</returns>
        MsgVpnDistributedCacheResponse CreateMsgVpnDistributedCache (MsgVpnDistributedCache body, string msgVpnName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Distributed Cache object.
        /// </summary>
        /// <remarks>
        /// Create a Distributed Cache object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x|x|||| msgVpnName|x||x|||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheResponse</returns>
        ApiResponse<MsgVpnDistributedCacheResponse> CreateMsgVpnDistributedCacheWithHttpInfo (MsgVpnDistributedCache body, string msgVpnName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Create a Cache Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x|x|||| msgVpnName|x||x|||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterResponse</returns>
        MsgVpnDistributedCacheClusterResponse CreateMsgVpnDistributedCacheCluster (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Create a Cache Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x|x|||| msgVpnName|x||x|||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterResponse</returns>
        ApiResponse<MsgVpnDistributedCacheClusterResponse> CreateMsgVpnDistributedCacheClusterWithHttpInfo (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Home Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Create a Home Cache Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| homeClusterName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Home Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse</returns>
        MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse CreateMsgVpnDistributedCacheClusterGlobalCachingHomeCluster (MsgVpnDistributedCacheClusterGlobalCachingHomeCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Home Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Create a Home Cache Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| homeClusterName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Home Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse</returns>
        ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse> CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterWithHttpInfo (MsgVpnDistributedCacheClusterGlobalCachingHomeCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Topic Prefix object.
        /// </summary>
        /// <remarks>
        /// Create a Topic Prefix object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| homeClusterName|x||x||| msgVpnName|x||x||| topicPrefix|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Topic Prefix object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse</returns>
        MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix body, string msgVpnName, string cacheName, string clusterName, string homeClusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Topic Prefix object.
        /// </summary>
        /// <remarks>
        /// Create a Topic Prefix object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| homeClusterName|x||x||| msgVpnName|x||x||| topicPrefix|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Topic Prefix object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse</returns>
        ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse> CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixWithHttpInfo (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix body, string msgVpnName, string cacheName, string clusterName, string homeClusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Cache Instance object.
        /// </summary>
        /// <remarks>
        /// Create a Cache Instance object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| instanceName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterInstanceResponse</returns>
        MsgVpnDistributedCacheClusterInstanceResponse CreateMsgVpnDistributedCacheClusterInstance (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Cache Instance object.
        /// </summary>
        /// <remarks>
        /// Create a Cache Instance object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| instanceName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterInstanceResponse</returns>
        ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse> CreateMsgVpnDistributedCacheClusterInstanceWithHttpInfo (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Topic object.
        /// </summary>
        /// <remarks>
        /// Create a Topic object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| msgVpnName|x||x||| topic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Topic object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterTopicResponse</returns>
        MsgVpnDistributedCacheClusterTopicResponse CreateMsgVpnDistributedCacheClusterTopic (MsgVpnDistributedCacheClusterTopic body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Topic object.
        /// </summary>
        /// <remarks>
        /// Create a Topic object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| msgVpnName|x||x||| topic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Topic object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterTopicResponse</returns>
        ApiResponse<MsgVpnDistributedCacheClusterTopicResponse> CreateMsgVpnDistributedCacheClusterTopicWithHttpInfo (MsgVpnDistributedCacheClusterTopic body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Delete a Distributed Cache object.
        /// </summary>
        /// <remarks>
        /// Delete a Distributed Cache object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnDistributedCache (string msgVpnName, string cacheName);

        /// <summary>
        /// Delete a Distributed Cache object.
        /// </summary>
        /// <remarks>
        /// Delete a Distributed Cache object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnDistributedCacheWithHttpInfo (string msgVpnName, string cacheName);
        /// <summary>
        /// Delete a Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Delete a Cache Cluster object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnDistributedCacheCluster (string msgVpnName, string cacheName, string clusterName);

        /// <summary>
        /// Delete a Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Delete a Cache Cluster object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnDistributedCacheClusterWithHttpInfo (string msgVpnName, string cacheName, string clusterName);
        /// <summary>
        /// Delete a Home Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Delete a Home Cache Cluster object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeCluster (string msgVpnName, string cacheName, string clusterName, string homeClusterName);

        /// <summary>
        /// Delete a Home Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Delete a Home Cache Cluster object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string homeClusterName);
        /// <summary>
        /// Delete a Topic Prefix object.
        /// </summary>
        /// <remarks>
        /// Delete a Topic Prefix object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="topicPrefix">A topic prefix for global topics available from the remote Home Cache Cluster. A wildcard (/&gt;) is implied at the end of the prefix.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string topicPrefix);

        /// <summary>
        /// Delete a Topic Prefix object.
        /// </summary>
        /// <remarks>
        /// Delete a Topic Prefix object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="topicPrefix">A topic prefix for global topics available from the remote Home Cache Cluster. A wildcard (/&gt;) is implied at the end of the prefix.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string topicPrefix);
        /// <summary>
        /// Delete a Cache Instance object.
        /// </summary>
        /// <remarks>
        /// Delete a Cache Instance object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnDistributedCacheClusterInstance (string msgVpnName, string cacheName, string clusterName, string instanceName);

        /// <summary>
        /// Delete a Cache Instance object.
        /// </summary>
        /// <remarks>
        /// Delete a Cache Instance object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnDistributedCacheClusterInstanceWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string instanceName);
        /// <summary>
        /// Delete a Topic object.
        /// </summary>
        /// <remarks>
        /// Delete a Topic object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="topic">The value of the Topic in the form a/b/c.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnDistributedCacheClusterTopic (string msgVpnName, string cacheName, string clusterName, string topic);

        /// <summary>
        /// Delete a Topic object.
        /// </summary>
        /// <remarks>
        /// Delete a Topic object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="topic">The value of the Topic in the form a/b/c.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnDistributedCacheClusterTopicWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string topic);
        /// <summary>
        /// Get a Distributed Cache object.
        /// </summary>
        /// <remarks>
        /// Get a Distributed Cache object.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheResponse</returns>
        MsgVpnDistributedCacheResponse GetMsgVpnDistributedCache (string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Distributed Cache object.
        /// </summary>
        /// <remarks>
        /// Get a Distributed Cache object.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheResponse</returns>
        ApiResponse<MsgVpnDistributedCacheResponse> GetMsgVpnDistributedCacheWithHttpInfo (string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Get a Cache Cluster object.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterResponse</returns>
        MsgVpnDistributedCacheClusterResponse GetMsgVpnDistributedCacheCluster (string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Get a Cache Cluster object.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterResponse</returns>
        ApiResponse<MsgVpnDistributedCacheClusterResponse> GetMsgVpnDistributedCacheClusterWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Home Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Get a Home Cache Cluster object.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse</returns>
        MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse GetMsgVpnDistributedCacheClusterGlobalCachingHomeCluster (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Home Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Get a Home Cache Cluster object.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse</returns>
        ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse> GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Topic Prefix object.
        /// </summary>
        /// <remarks>
        /// Get a Topic Prefix object.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x||| topicPrefix|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="topicPrefix">A topic prefix for global topics available from the remote Home Cache Cluster. A wildcard (/&gt;) is implied at the end of the prefix.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse</returns>
        MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string topicPrefix, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Topic Prefix object.
        /// </summary>
        /// <remarks>
        /// Get a Topic Prefix object.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x||| topicPrefix|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="topicPrefix">A topic prefix for global topics available from the remote Home Cache Cluster. A wildcard (/&gt;) is implied at the end of the prefix.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse</returns>
        ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse> GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string topicPrefix, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Topic Prefix objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Topic Prefix objects.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x||| topicPrefix|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse</returns>
        MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixes (string msgVpnName, string cacheName, string clusterName, string homeClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Topic Prefix objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Topic Prefix objects.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x||| topicPrefix|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse</returns>
        ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse> GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string homeClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of Home Cache Cluster objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Home Cache Cluster objects.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse</returns>
        MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusters (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Home Cache Cluster objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Home Cache Cluster objects.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse</returns>
        ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse> GetMsgVpnDistributedCacheClusterGlobalCachingHomeClustersWithHttpInfo (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Cache Instance object.
        /// </summary>
        /// <remarks>
        /// Get a Cache Instance object.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| instanceName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterInstanceResponse</returns>
        MsgVpnDistributedCacheClusterInstanceResponse GetMsgVpnDistributedCacheClusterInstance (string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Cache Instance object.
        /// </summary>
        /// <remarks>
        /// Get a Cache Instance object.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| instanceName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterInstanceResponse</returns>
        ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse> GetMsgVpnDistributedCacheClusterInstanceWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Cache Instance objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Cache Instance objects.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| instanceName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterInstancesResponse</returns>
        MsgVpnDistributedCacheClusterInstancesResponse GetMsgVpnDistributedCacheClusterInstances (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Cache Instance objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Cache Instance objects.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| instanceName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterInstancesResponse</returns>
        ApiResponse<MsgVpnDistributedCacheClusterInstancesResponse> GetMsgVpnDistributedCacheClusterInstancesWithHttpInfo (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Topic object.
        /// </summary>
        /// <remarks>
        /// Get a Topic object.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x||| topic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="topic">The value of the Topic in the form a/b/c.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterTopicResponse</returns>
        MsgVpnDistributedCacheClusterTopicResponse GetMsgVpnDistributedCacheClusterTopic (string msgVpnName, string cacheName, string clusterName, string topic, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Topic object.
        /// </summary>
        /// <remarks>
        /// Get a Topic object.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x||| topic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="topic">The value of the Topic in the form a/b/c.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterTopicResponse</returns>
        ApiResponse<MsgVpnDistributedCacheClusterTopicResponse> GetMsgVpnDistributedCacheClusterTopicWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string topic, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Topic objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Topic objects.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x||| topic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterTopicsResponse</returns>
        MsgVpnDistributedCacheClusterTopicsResponse GetMsgVpnDistributedCacheClusterTopics (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Topic objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Topic objects.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x||| topic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterTopicsResponse</returns>
        ApiResponse<MsgVpnDistributedCacheClusterTopicsResponse> GetMsgVpnDistributedCacheClusterTopicsWithHttpInfo (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of Cache Cluster objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Cache Cluster objects.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClustersResponse</returns>
        MsgVpnDistributedCacheClustersResponse GetMsgVpnDistributedCacheClusters (string msgVpnName, string cacheName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Cache Cluster objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Cache Cluster objects.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClustersResponse</returns>
        ApiResponse<MsgVpnDistributedCacheClustersResponse> GetMsgVpnDistributedCacheClustersWithHttpInfo (string msgVpnName, string cacheName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of Distributed Cache objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Distributed Cache objects.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCachesResponse</returns>
        MsgVpnDistributedCachesResponse GetMsgVpnDistributedCaches (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Distributed Cache objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Distributed Cache objects.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCachesResponse</returns>
        ApiResponse<MsgVpnDistributedCachesResponse> GetMsgVpnDistributedCachesWithHttpInfo (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Replace a Distributed Cache object.
        /// </summary>
        /// <remarks>
        /// Replace a Distributed Cache object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| cacheVirtualRouter||x|||||| msgVpnName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheResponse</returns>
        MsgVpnDistributedCacheResponse ReplaceMsgVpnDistributedCache (MsgVpnDistributedCache body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Distributed Cache object.
        /// </summary>
        /// <remarks>
        /// Replace a Distributed Cache object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| cacheVirtualRouter||x|||||| msgVpnName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheResponse</returns>
        ApiResponse<MsgVpnDistributedCacheResponse> ReplaceMsgVpnDistributedCacheWithHttpInfo (MsgVpnDistributedCache body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Replace a Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Replace a Cache Cluster object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| clusterName|x||x||||| deliverToOneOverrideEnabled||||||x|| msgVpnName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterResponse</returns>
        MsgVpnDistributedCacheClusterResponse ReplaceMsgVpnDistributedCacheCluster (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Replace a Cache Cluster object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| clusterName|x||x||||| deliverToOneOverrideEnabled||||||x|| msgVpnName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterResponse</returns>
        ApiResponse<MsgVpnDistributedCacheClusterResponse> ReplaceMsgVpnDistributedCacheClusterWithHttpInfo (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Replace a Cache Instance object.
        /// </summary>
        /// <remarks>
        /// Replace a Cache Instance object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| clusterName|x||x||||| instanceName|x||x||||| msgVpnName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterInstanceResponse</returns>
        MsgVpnDistributedCacheClusterInstanceResponse ReplaceMsgVpnDistributedCacheClusterInstance (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Cache Instance object.
        /// </summary>
        /// <remarks>
        /// Replace a Cache Instance object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| clusterName|x||x||||| instanceName|x||x||||| msgVpnName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterInstanceResponse</returns>
        ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse> ReplaceMsgVpnDistributedCacheClusterInstanceWithHttpInfo (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Distributed Cache object.
        /// </summary>
        /// <remarks>
        /// Update a Distributed Cache object. Any attribute missing from the request will be left unchanged.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| cacheVirtualRouter||x||||| msgVpnName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheResponse</returns>
        MsgVpnDistributedCacheResponse UpdateMsgVpnDistributedCache (MsgVpnDistributedCache body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Distributed Cache object.
        /// </summary>
        /// <remarks>
        /// Update a Distributed Cache object. Any attribute missing from the request will be left unchanged.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| cacheVirtualRouter||x||||| msgVpnName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheResponse</returns>
        ApiResponse<MsgVpnDistributedCacheResponse> UpdateMsgVpnDistributedCacheWithHttpInfo (MsgVpnDistributedCache body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Update a Cache Cluster object. Any attribute missing from the request will be left unchanged.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| clusterName|x|x||||| deliverToOneOverrideEnabled|||||x|| msgVpnName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterResponse</returns>
        MsgVpnDistributedCacheClusterResponse UpdateMsgVpnDistributedCacheCluster (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Update a Cache Cluster object. Any attribute missing from the request will be left unchanged.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| clusterName|x|x||||| deliverToOneOverrideEnabled|||||x|| msgVpnName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterResponse</returns>
        ApiResponse<MsgVpnDistributedCacheClusterResponse> UpdateMsgVpnDistributedCacheClusterWithHttpInfo (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Cache Instance object.
        /// </summary>
        /// <remarks>
        /// Update a Cache Instance object. Any attribute missing from the request will be left unchanged.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| clusterName|x|x||||| instanceName|x|x||||| msgVpnName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterInstanceResponse</returns>
        MsgVpnDistributedCacheClusterInstanceResponse UpdateMsgVpnDistributedCacheClusterInstance (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Cache Instance object.
        /// </summary>
        /// <remarks>
        /// Update a Cache Instance object. Any attribute missing from the request will be left unchanged.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| clusterName|x|x||||| instanceName|x|x||||| msgVpnName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterInstanceResponse</returns>
        ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse> UpdateMsgVpnDistributedCacheClusterInstanceWithHttpInfo (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null);
        #endregion Synchronous Operations
        #region Asynchronous Operations
        /// <summary>
        /// Create a Distributed Cache object.
        /// </summary>
        /// <remarks>
        /// Create a Distributed Cache object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x|x|||| msgVpnName|x||x|||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheResponse> CreateMsgVpnDistributedCacheAsync (MsgVpnDistributedCache body, string msgVpnName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Distributed Cache object.
        /// </summary>
        /// <remarks>
        /// Create a Distributed Cache object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x|x|||| msgVpnName|x||x|||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheResponse>> CreateMsgVpnDistributedCacheAsyncWithHttpInfo (MsgVpnDistributedCache body, string msgVpnName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Create a Cache Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x|x|||| msgVpnName|x||x|||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterResponse> CreateMsgVpnDistributedCacheClusterAsync (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Create a Cache Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x|x|||| msgVpnName|x||x|||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterResponse>> CreateMsgVpnDistributedCacheClusterAsyncWithHttpInfo (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Home Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Create a Home Cache Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| homeClusterName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Home Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse> CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterAsync (MsgVpnDistributedCacheClusterGlobalCachingHomeCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Home Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Create a Home Cache Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| homeClusterName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Home Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse>> CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterAsyncWithHttpInfo (MsgVpnDistributedCacheClusterGlobalCachingHomeCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Topic Prefix object.
        /// </summary>
        /// <remarks>
        /// Create a Topic Prefix object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| homeClusterName|x||x||| msgVpnName|x||x||| topicPrefix|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Topic Prefix object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse> CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixAsync (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix body, string msgVpnName, string cacheName, string clusterName, string homeClusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Topic Prefix object.
        /// </summary>
        /// <remarks>
        /// Create a Topic Prefix object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| homeClusterName|x||x||| msgVpnName|x||x||| topicPrefix|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Topic Prefix object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse>> CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixAsyncWithHttpInfo (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix body, string msgVpnName, string cacheName, string clusterName, string homeClusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Cache Instance object.
        /// </summary>
        /// <remarks>
        /// Create a Cache Instance object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| instanceName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterInstanceResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterInstanceResponse> CreateMsgVpnDistributedCacheClusterInstanceAsync (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Cache Instance object.
        /// </summary>
        /// <remarks>
        /// Create a Cache Instance object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| instanceName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterInstanceResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse>> CreateMsgVpnDistributedCacheClusterInstanceAsyncWithHttpInfo (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Topic object.
        /// </summary>
        /// <remarks>
        /// Create a Topic object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| msgVpnName|x||x||| topic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Topic object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterTopicResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterTopicResponse> CreateMsgVpnDistributedCacheClusterTopicAsync (MsgVpnDistributedCacheClusterTopic body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Topic object.
        /// </summary>
        /// <remarks>
        /// Create a Topic object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| msgVpnName|x||x||| topic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Topic object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterTopicResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterTopicResponse>> CreateMsgVpnDistributedCacheClusterTopicAsyncWithHttpInfo (MsgVpnDistributedCacheClusterTopic body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Delete a Distributed Cache object.
        /// </summary>
        /// <remarks>
        /// Delete a Distributed Cache object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnDistributedCacheAsync (string msgVpnName, string cacheName);

        /// <summary>
        /// Delete a Distributed Cache object.
        /// </summary>
        /// <remarks>
        /// Delete a Distributed Cache object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnDistributedCacheAsyncWithHttpInfo (string msgVpnName, string cacheName);
        /// <summary>
        /// Delete a Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Delete a Cache Cluster object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnDistributedCacheClusterAsync (string msgVpnName, string cacheName, string clusterName);

        /// <summary>
        /// Delete a Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Delete a Cache Cluster object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnDistributedCacheClusterAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName);
        /// <summary>
        /// Delete a Home Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Delete a Home Cache Cluster object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterAsync (string msgVpnName, string cacheName, string clusterName, string homeClusterName);

        /// <summary>
        /// Delete a Home Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Delete a Home Cache Cluster object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string homeClusterName);
        /// <summary>
        /// Delete a Topic Prefix object.
        /// </summary>
        /// <remarks>
        /// Delete a Topic Prefix object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="topicPrefix">A topic prefix for global topics available from the remote Home Cache Cluster. A wildcard (/&gt;) is implied at the end of the prefix.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixAsync (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string topicPrefix);

        /// <summary>
        /// Delete a Topic Prefix object.
        /// </summary>
        /// <remarks>
        /// Delete a Topic Prefix object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="topicPrefix">A topic prefix for global topics available from the remote Home Cache Cluster. A wildcard (/&gt;) is implied at the end of the prefix.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string topicPrefix);
        /// <summary>
        /// Delete a Cache Instance object.
        /// </summary>
        /// <remarks>
        /// Delete a Cache Instance object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnDistributedCacheClusterInstanceAsync (string msgVpnName, string cacheName, string clusterName, string instanceName);

        /// <summary>
        /// Delete a Cache Instance object.
        /// </summary>
        /// <remarks>
        /// Delete a Cache Instance object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnDistributedCacheClusterInstanceAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string instanceName);
        /// <summary>
        /// Delete a Topic object.
        /// </summary>
        /// <remarks>
        /// Delete a Topic object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="topic">The value of the Topic in the form a/b/c.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnDistributedCacheClusterTopicAsync (string msgVpnName, string cacheName, string clusterName, string topic);

        /// <summary>
        /// Delete a Topic object.
        /// </summary>
        /// <remarks>
        /// Delete a Topic object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="topic">The value of the Topic in the form a/b/c.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnDistributedCacheClusterTopicAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string topic);
        /// <summary>
        /// Get a Distributed Cache object.
        /// </summary>
        /// <remarks>
        /// Get a Distributed Cache object.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheResponse> GetMsgVpnDistributedCacheAsync (string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Distributed Cache object.
        /// </summary>
        /// <remarks>
        /// Get a Distributed Cache object.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheResponse>> GetMsgVpnDistributedCacheAsyncWithHttpInfo (string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Get a Cache Cluster object.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterResponse> GetMsgVpnDistributedCacheClusterAsync (string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Get a Cache Cluster object.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterResponse>> GetMsgVpnDistributedCacheClusterAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Home Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Get a Home Cache Cluster object.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse> GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterAsync (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Home Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Get a Home Cache Cluster object.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse>> GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Topic Prefix object.
        /// </summary>
        /// <remarks>
        /// Get a Topic Prefix object.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x||| topicPrefix|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="topicPrefix">A topic prefix for global topics available from the remote Home Cache Cluster. A wildcard (/&gt;) is implied at the end of the prefix.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse> GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixAsync (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string topicPrefix, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Topic Prefix object.
        /// </summary>
        /// <remarks>
        /// Get a Topic Prefix object.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x||| topicPrefix|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="topicPrefix">A topic prefix for global topics available from the remote Home Cache Cluster. A wildcard (/&gt;) is implied at the end of the prefix.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse>> GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string topicPrefix, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Topic Prefix objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Topic Prefix objects.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x||| topicPrefix|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse> GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesAsync (string msgVpnName, string cacheName, string clusterName, string homeClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Topic Prefix objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Topic Prefix objects.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x||| topicPrefix|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse>> GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string homeClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of Home Cache Cluster objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Home Cache Cluster objects.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse> GetMsgVpnDistributedCacheClusterGlobalCachingHomeClustersAsync (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Home Cache Cluster objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Home Cache Cluster objects.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse>> GetMsgVpnDistributedCacheClusterGlobalCachingHomeClustersAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Cache Instance object.
        /// </summary>
        /// <remarks>
        /// Get a Cache Instance object.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| instanceName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterInstanceResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterInstanceResponse> GetMsgVpnDistributedCacheClusterInstanceAsync (string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Cache Instance object.
        /// </summary>
        /// <remarks>
        /// Get a Cache Instance object.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| instanceName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterInstanceResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse>> GetMsgVpnDistributedCacheClusterInstanceAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Cache Instance objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Cache Instance objects.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| instanceName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterInstancesResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterInstancesResponse> GetMsgVpnDistributedCacheClusterInstancesAsync (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Cache Instance objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Cache Instance objects.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| instanceName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterInstancesResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterInstancesResponse>> GetMsgVpnDistributedCacheClusterInstancesAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Topic object.
        /// </summary>
        /// <remarks>
        /// Get a Topic object.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x||| topic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="topic">The value of the Topic in the form a/b/c.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterTopicResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterTopicResponse> GetMsgVpnDistributedCacheClusterTopicAsync (string msgVpnName, string cacheName, string clusterName, string topic, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Topic object.
        /// </summary>
        /// <remarks>
        /// Get a Topic object.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x||| topic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="topic">The value of the Topic in the form a/b/c.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterTopicResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterTopicResponse>> GetMsgVpnDistributedCacheClusterTopicAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string topic, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Topic objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Topic objects.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x||| topic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterTopicsResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterTopicsResponse> GetMsgVpnDistributedCacheClusterTopicsAsync (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Topic objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Topic objects.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x||| topic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterTopicsResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterTopicsResponse>> GetMsgVpnDistributedCacheClusterTopicsAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of Cache Cluster objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Cache Cluster objects.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClustersResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheClustersResponse> GetMsgVpnDistributedCacheClustersAsync (string msgVpnName, string cacheName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Cache Cluster objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Cache Cluster objects.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClustersResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClustersResponse>> GetMsgVpnDistributedCacheClustersAsyncWithHttpInfo (string msgVpnName, string cacheName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of Distributed Cache objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Distributed Cache objects.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCachesResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCachesResponse> GetMsgVpnDistributedCachesAsync (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Distributed Cache objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Distributed Cache objects.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCachesResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCachesResponse>> GetMsgVpnDistributedCachesAsyncWithHttpInfo (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Replace a Distributed Cache object.
        /// </summary>
        /// <remarks>
        /// Replace a Distributed Cache object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| cacheVirtualRouter||x|||||| msgVpnName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheResponse> ReplaceMsgVpnDistributedCacheAsync (MsgVpnDistributedCache body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Distributed Cache object.
        /// </summary>
        /// <remarks>
        /// Replace a Distributed Cache object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| cacheVirtualRouter||x|||||| msgVpnName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheResponse>> ReplaceMsgVpnDistributedCacheAsyncWithHttpInfo (MsgVpnDistributedCache body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Replace a Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Replace a Cache Cluster object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| clusterName|x||x||||| deliverToOneOverrideEnabled||||||x|| msgVpnName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterResponse> ReplaceMsgVpnDistributedCacheClusterAsync (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Replace a Cache Cluster object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| clusterName|x||x||||| deliverToOneOverrideEnabled||||||x|| msgVpnName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterResponse>> ReplaceMsgVpnDistributedCacheClusterAsyncWithHttpInfo (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Replace a Cache Instance object.
        /// </summary>
        /// <remarks>
        /// Replace a Cache Instance object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| clusterName|x||x||||| instanceName|x||x||||| msgVpnName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterInstanceResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterInstanceResponse> ReplaceMsgVpnDistributedCacheClusterInstanceAsync (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Cache Instance object.
        /// </summary>
        /// <remarks>
        /// Replace a Cache Instance object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| clusterName|x||x||||| instanceName|x||x||||| msgVpnName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterInstanceResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse>> ReplaceMsgVpnDistributedCacheClusterInstanceAsyncWithHttpInfo (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Distributed Cache object.
        /// </summary>
        /// <remarks>
        /// Update a Distributed Cache object. Any attribute missing from the request will be left unchanged.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| cacheVirtualRouter||x||||| msgVpnName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheResponse> UpdateMsgVpnDistributedCacheAsync (MsgVpnDistributedCache body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Distributed Cache object.
        /// </summary>
        /// <remarks>
        /// Update a Distributed Cache object. Any attribute missing from the request will be left unchanged.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| cacheVirtualRouter||x||||| msgVpnName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheResponse>> UpdateMsgVpnDistributedCacheAsyncWithHttpInfo (MsgVpnDistributedCache body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Update a Cache Cluster object. Any attribute missing from the request will be left unchanged.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| clusterName|x|x||||| deliverToOneOverrideEnabled|||||x|| msgVpnName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterResponse> UpdateMsgVpnDistributedCacheClusterAsync (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Cache Cluster object.
        /// </summary>
        /// <remarks>
        /// Update a Cache Cluster object. Any attribute missing from the request will be left unchanged.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| clusterName|x|x||||| deliverToOneOverrideEnabled|||||x|| msgVpnName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterResponse>> UpdateMsgVpnDistributedCacheClusterAsyncWithHttpInfo (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Cache Instance object.
        /// </summary>
        /// <remarks>
        /// Update a Cache Instance object. Any attribute missing from the request will be left unchanged.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| clusterName|x|x||||| instanceName|x|x||||| msgVpnName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterInstanceResponse</returns>
        System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterInstanceResponse> UpdateMsgVpnDistributedCacheClusterInstanceAsync (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Cache Instance object.
        /// </summary>
        /// <remarks>
        /// Update a Cache Instance object. Any attribute missing from the request will be left unchanged.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| clusterName|x|x||||| instanceName|x|x||||| msgVpnName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterInstanceResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse>> UpdateMsgVpnDistributedCacheClusterInstanceAsyncWithHttpInfo (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null);
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
        public partial class DistributedCacheApi : IDistributedCacheApi
    {
        private Semp.V2.CSharp.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedCacheApi"/> class.
        /// </summary>
        /// <returns></returns>
        public DistributedCacheApi(String basePath)
        {
            this.Configuration = new Semp.V2.CSharp.Client.Configuration { BasePath = basePath };

            ExceptionFactory = Semp.V2.CSharp.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedCacheApi"/> class
        /// </summary>
        /// <returns></returns>
        public DistributedCacheApi()
        {
            this.Configuration = Semp.V2.CSharp.Client.Configuration.Default;

            ExceptionFactory = Semp.V2.CSharp.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedCacheApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public DistributedCacheApi(Semp.V2.CSharp.Client.Configuration configuration = null)
        {
            if (configuration == null) // use the default one in Configuration
                this.Configuration = Semp.V2.CSharp.Client.Configuration.Default;
            else
                this.Configuration = configuration;

            ExceptionFactory = Semp.V2.CSharp.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        public String GetBasePath()
        {
            return this.Configuration.ApiClient.RestClient.BaseUrl.ToString();
        }

        /// <summary>
        /// Sets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        [Obsolete("SetBasePath is deprecated, please do 'Configuration.ApiClient = new ApiClient(\"http://new-path\")' instead.")]
        public void SetBasePath(String basePath)
        {
            // do nothing
        }

        /// <summary>
        /// Gets or sets the configuration object
        /// </summary>
        /// <value>An instance of the Configuration</value>
        public Semp.V2.CSharp.Client.Configuration Configuration {get; set;}

        /// <summary>
        /// Provides a factory method hook for the creation of exceptions.
        /// </summary>
        public Semp.V2.CSharp.Client.ExceptionFactory ExceptionFactory
        {
            get
            {
                if (_exceptionFactory != null && _exceptionFactory.GetInvocationList().Length > 1)
                {
                    throw new InvalidOperationException("Multicast delegate for ExceptionFactory is unsupported.");
                }
                return _exceptionFactory;
            }
            set { _exceptionFactory = value; }
        }

        /// <summary>
        /// Gets the default header.
        /// </summary>
        /// <returns>Dictionary of HTTP header</returns>
        [Obsolete("DefaultHeader is deprecated, please use Configuration.DefaultHeader instead.")]
        public IDictionary<String, String> DefaultHeader()
        {
            return new ReadOnlyDictionary<string, string>(this.Configuration.DefaultHeader);
        }

        /// <summary>
        /// Add default header.
        /// </summary>
        /// <param name="key">Header field name.</param>
        /// <param name="value">Header field value.</param>
        /// <returns></returns>
        [Obsolete("AddDefaultHeader is deprecated, please use Configuration.AddDefaultHeader instead.")]
        public void AddDefaultHeader(string key, string value)
        {
            this.Configuration.AddDefaultHeader(key, value);
        }

        /// <summary>
        /// Create a Distributed Cache object. Create a Distributed Cache object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x|x|||| msgVpnName|x||x|||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheResponse</returns>
        public MsgVpnDistributedCacheResponse CreateMsgVpnDistributedCache (MsgVpnDistributedCache body, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheResponse> localVarResponse = CreateMsgVpnDistributedCacheWithHttpInfo(body, msgVpnName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Distributed Cache object. Create a Distributed Cache object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x|x|||| msgVpnName|x||x|||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheResponse > CreateMsgVpnDistributedCacheWithHttpInfo (MsgVpnDistributedCache body, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->CreateMsgVpnDistributedCache");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->CreateMsgVpnDistributedCache");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateMsgVpnDistributedCache", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheResponse)));
        }

        /// <summary>
        /// Create a Distributed Cache object. Create a Distributed Cache object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x|x|||| msgVpnName|x||x|||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheResponse> CreateMsgVpnDistributedCacheAsync (MsgVpnDistributedCache body, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheResponse> localVarResponse = await CreateMsgVpnDistributedCacheAsyncWithHttpInfo(body, msgVpnName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Distributed Cache object. Create a Distributed Cache object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x|x|||| msgVpnName|x||x|||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheResponse>> CreateMsgVpnDistributedCacheAsyncWithHttpInfo (MsgVpnDistributedCache body, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->CreateMsgVpnDistributedCache");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->CreateMsgVpnDistributedCache");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateMsgVpnDistributedCache", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheResponse)));
        }

        /// <summary>
        /// Create a Cache Cluster object. Create a Cache Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x|x|||| msgVpnName|x||x|||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterResponse</returns>
        public MsgVpnDistributedCacheClusterResponse CreateMsgVpnDistributedCacheCluster (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterResponse> localVarResponse = CreateMsgVpnDistributedCacheClusterWithHttpInfo(body, msgVpnName, cacheName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Cache Cluster object. Create a Cache Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x|x|||| msgVpnName|x||x|||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheClusterResponse > CreateMsgVpnDistributedCacheClusterWithHttpInfo (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheCluster");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheCluster");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheCluster");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateMsgVpnDistributedCacheCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterResponse)));
        }

        /// <summary>
        /// Create a Cache Cluster object. Create a Cache Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x|x|||| msgVpnName|x||x|||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterResponse> CreateMsgVpnDistributedCacheClusterAsync (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterResponse> localVarResponse = await CreateMsgVpnDistributedCacheClusterAsyncWithHttpInfo(body, msgVpnName, cacheName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Cache Cluster object. Create a Cache Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x|x|||| msgVpnName|x||x|||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterResponse>> CreateMsgVpnDistributedCacheClusterAsyncWithHttpInfo (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheCluster");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheCluster");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheCluster");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateMsgVpnDistributedCacheCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterResponse)));
        }

        /// <summary>
        /// Create a Home Cache Cluster object. Create a Home Cache Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| homeClusterName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Home Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse</returns>
        public MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse CreateMsgVpnDistributedCacheClusterGlobalCachingHomeCluster (MsgVpnDistributedCacheClusterGlobalCachingHomeCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse> localVarResponse = CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterWithHttpInfo(body, msgVpnName, cacheName, clusterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Home Cache Cluster object. Create a Home Cache Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| homeClusterName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Home Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse > CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterWithHttpInfo (MsgVpnDistributedCacheClusterGlobalCachingHomeCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateMsgVpnDistributedCacheClusterGlobalCachingHomeCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse)));
        }

        /// <summary>
        /// Create a Home Cache Cluster object. Create a Home Cache Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| homeClusterName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Home Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse> CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterAsync (MsgVpnDistributedCacheClusterGlobalCachingHomeCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse> localVarResponse = await CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterAsyncWithHttpInfo(body, msgVpnName, cacheName, clusterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Home Cache Cluster object. Create a Home Cache Cluster object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| homeClusterName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Home Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse>> CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterAsyncWithHttpInfo (MsgVpnDistributedCacheClusterGlobalCachingHomeCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateMsgVpnDistributedCacheClusterGlobalCachingHomeCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse)));
        }

        /// <summary>
        /// Create a Topic Prefix object. Create a Topic Prefix object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| homeClusterName|x||x||| msgVpnName|x||x||| topicPrefix|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Topic Prefix object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse</returns>
        public MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix body, string msgVpnName, string cacheName, string clusterName, string homeClusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse> localVarResponse = CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixWithHttpInfo(body, msgVpnName, cacheName, clusterName, homeClusterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Topic Prefix object. Create a Topic Prefix object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| homeClusterName|x||x||| msgVpnName|x||x||| topicPrefix|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Topic Prefix object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse > CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixWithHttpInfo (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix body, string msgVpnName, string cacheName, string clusterName, string homeClusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'homeClusterName' is set
            if (homeClusterName == null)
                throw new ApiException(400, "Missing required parameter 'homeClusterName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (homeClusterName != null) localVarPathParams.Add("homeClusterName", this.Configuration.ApiClient.ParameterToString(homeClusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse)));
        }

        /// <summary>
        /// Create a Topic Prefix object. Create a Topic Prefix object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| homeClusterName|x||x||| msgVpnName|x||x||| topicPrefix|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Topic Prefix object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse> CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixAsync (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix body, string msgVpnName, string cacheName, string clusterName, string homeClusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse> localVarResponse = await CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixAsyncWithHttpInfo(body, msgVpnName, cacheName, clusterName, homeClusterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Topic Prefix object. Create a Topic Prefix object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| homeClusterName|x||x||| msgVpnName|x||x||| topicPrefix|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Topic Prefix object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse>> CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixAsyncWithHttpInfo (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix body, string msgVpnName, string cacheName, string clusterName, string homeClusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'homeClusterName' is set
            if (homeClusterName == null)
                throw new ApiException(400, "Missing required parameter 'homeClusterName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (homeClusterName != null) localVarPathParams.Add("homeClusterName", this.Configuration.ApiClient.ParameterToString(homeClusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse)));
        }

        /// <summary>
        /// Create a Cache Instance object. Create a Cache Instance object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| instanceName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterInstanceResponse</returns>
        public MsgVpnDistributedCacheClusterInstanceResponse CreateMsgVpnDistributedCacheClusterInstance (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse> localVarResponse = CreateMsgVpnDistributedCacheClusterInstanceWithHttpInfo(body, msgVpnName, cacheName, clusterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Cache Instance object. Create a Cache Instance object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| instanceName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterInstanceResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheClusterInstanceResponse > CreateMsgVpnDistributedCacheClusterInstanceWithHttpInfo (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterInstance");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateMsgVpnDistributedCacheClusterInstance", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterInstanceResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterInstanceResponse)));
        }

        /// <summary>
        /// Create a Cache Instance object. Create a Cache Instance object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| instanceName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterInstanceResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterInstanceResponse> CreateMsgVpnDistributedCacheClusterInstanceAsync (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse> localVarResponse = await CreateMsgVpnDistributedCacheClusterInstanceAsyncWithHttpInfo(body, msgVpnName, cacheName, clusterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Cache Instance object. Create a Cache Instance object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| instanceName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterInstanceResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse>> CreateMsgVpnDistributedCacheClusterInstanceAsyncWithHttpInfo (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterInstance");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateMsgVpnDistributedCacheClusterInstance", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterInstanceResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterInstanceResponse)));
        }

        /// <summary>
        /// Create a Topic object. Create a Topic object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| msgVpnName|x||x||| topic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Topic object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterTopicResponse</returns>
        public MsgVpnDistributedCacheClusterTopicResponse CreateMsgVpnDistributedCacheClusterTopic (MsgVpnDistributedCacheClusterTopic body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterTopicResponse> localVarResponse = CreateMsgVpnDistributedCacheClusterTopicWithHttpInfo(body, msgVpnName, cacheName, clusterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Topic object. Create a Topic object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| msgVpnName|x||x||| topic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Topic object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterTopicResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheClusterTopicResponse > CreateMsgVpnDistributedCacheClusterTopicWithHttpInfo (MsgVpnDistributedCacheClusterTopic body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterTopic");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterTopic");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterTopic");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterTopic");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateMsgVpnDistributedCacheClusterTopic", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterTopicResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterTopicResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterTopicResponse)));
        }

        /// <summary>
        /// Create a Topic object. Create a Topic object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| msgVpnName|x||x||| topic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Topic object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterTopicResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterTopicResponse> CreateMsgVpnDistributedCacheClusterTopicAsync (MsgVpnDistributedCacheClusterTopic body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterTopicResponse> localVarResponse = await CreateMsgVpnDistributedCacheClusterTopicAsyncWithHttpInfo(body, msgVpnName, cacheName, clusterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Topic object. Create a Topic object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: cacheName|x||x||| clusterName|x||x||| msgVpnName|x||x||| topic|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Topic object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterTopicResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterTopicResponse>> CreateMsgVpnDistributedCacheClusterTopicAsyncWithHttpInfo (MsgVpnDistributedCacheClusterTopic body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterTopic");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterTopic");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterTopic");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->CreateMsgVpnDistributedCacheClusterTopic");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateMsgVpnDistributedCacheClusterTopic", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterTopicResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterTopicResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterTopicResponse)));
        }

        /// <summary>
        /// Delete a Distributed Cache object. Delete a Distributed Cache object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnDistributedCache (string msgVpnName, string cacheName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnDistributedCacheWithHttpInfo(msgVpnName, cacheName);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Distributed Cache object. Delete a Distributed Cache object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnDistributedCacheWithHttpInfo (string msgVpnName, string cacheName)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCache");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCache");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteMsgVpnDistributedCache", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Distributed Cache object. Delete a Distributed Cache object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnDistributedCacheAsync (string msgVpnName, string cacheName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnDistributedCacheAsyncWithHttpInfo(msgVpnName, cacheName);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Distributed Cache object. Delete a Distributed Cache object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnDistributedCacheAsyncWithHttpInfo (string msgVpnName, string cacheName)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCache");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCache");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteMsgVpnDistributedCache", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Cache Cluster object. Delete a Cache Cluster object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnDistributedCacheCluster (string msgVpnName, string cacheName, string clusterName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnDistributedCacheClusterWithHttpInfo(msgVpnName, cacheName, clusterName);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Cache Cluster object. Delete a Cache Cluster object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnDistributedCacheClusterWithHttpInfo (string msgVpnName, string cacheName, string clusterName)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheCluster");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheCluster");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheCluster");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteMsgVpnDistributedCacheCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Cache Cluster object. Delete a Cache Cluster object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnDistributedCacheClusterAsync (string msgVpnName, string cacheName, string clusterName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnDistributedCacheClusterAsyncWithHttpInfo(msgVpnName, cacheName, clusterName);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Cache Cluster object. Delete a Cache Cluster object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnDistributedCacheClusterAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheCluster");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheCluster");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheCluster");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteMsgVpnDistributedCacheCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Home Cache Cluster object. Delete a Home Cache Cluster object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeCluster (string msgVpnName, string cacheName, string clusterName, string homeClusterName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterWithHttpInfo(msgVpnName, cacheName, clusterName, homeClusterName);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Home Cache Cluster object. Delete a Home Cache Cluster object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string homeClusterName)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");
            // verify the required parameter 'homeClusterName' is set
            if (homeClusterName == null)
                throw new ApiException(400, "Missing required parameter 'homeClusterName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (homeClusterName != null) localVarPathParams.Add("homeClusterName", this.Configuration.ApiClient.ParameterToString(homeClusterName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Home Cache Cluster object. Delete a Home Cache Cluster object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterAsync (string msgVpnName, string cacheName, string clusterName, string homeClusterName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterAsyncWithHttpInfo(msgVpnName, cacheName, clusterName, homeClusterName);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Home Cache Cluster object. Delete a Home Cache Cluster object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string homeClusterName)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");
            // verify the required parameter 'homeClusterName' is set
            if (homeClusterName == null)
                throw new ApiException(400, "Missing required parameter 'homeClusterName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (homeClusterName != null) localVarPathParams.Add("homeClusterName", this.Configuration.ApiClient.ParameterToString(homeClusterName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Topic Prefix object. Delete a Topic Prefix object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="topicPrefix">A topic prefix for global topics available from the remote Home Cache Cluster. A wildcard (/&gt;) is implied at the end of the prefix.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string topicPrefix)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixWithHttpInfo(msgVpnName, cacheName, clusterName, homeClusterName, topicPrefix);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Topic Prefix object. Delete a Topic Prefix object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="topicPrefix">A topic prefix for global topics available from the remote Home Cache Cluster. A wildcard (/&gt;) is implied at the end of the prefix.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string topicPrefix)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'homeClusterName' is set
            if (homeClusterName == null)
                throw new ApiException(400, "Missing required parameter 'homeClusterName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'topicPrefix' is set
            if (topicPrefix == null)
                throw new ApiException(400, "Missing required parameter 'topicPrefix' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes/{topicPrefix}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (homeClusterName != null) localVarPathParams.Add("homeClusterName", this.Configuration.ApiClient.ParameterToString(homeClusterName)); // path parameter
            if (topicPrefix != null) localVarPathParams.Add("topicPrefix", this.Configuration.ApiClient.ParameterToString(topicPrefix)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Topic Prefix object. Delete a Topic Prefix object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="topicPrefix">A topic prefix for global topics available from the remote Home Cache Cluster. A wildcard (/&gt;) is implied at the end of the prefix.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixAsync (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string topicPrefix)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixAsyncWithHttpInfo(msgVpnName, cacheName, clusterName, homeClusterName, topicPrefix);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Topic Prefix object. Delete a Topic Prefix object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="topicPrefix">A topic prefix for global topics available from the remote Home Cache Cluster. A wildcard (/&gt;) is implied at the end of the prefix.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string topicPrefix)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'homeClusterName' is set
            if (homeClusterName == null)
                throw new ApiException(400, "Missing required parameter 'homeClusterName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'topicPrefix' is set
            if (topicPrefix == null)
                throw new ApiException(400, "Missing required parameter 'topicPrefix' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes/{topicPrefix}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (homeClusterName != null) localVarPathParams.Add("homeClusterName", this.Configuration.ApiClient.ParameterToString(homeClusterName)); // path parameter
            if (topicPrefix != null) localVarPathParams.Add("topicPrefix", this.Configuration.ApiClient.ParameterToString(topicPrefix)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Cache Instance object. Delete a Cache Instance object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnDistributedCacheClusterInstance (string msgVpnName, string cacheName, string clusterName, string instanceName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnDistributedCacheClusterInstanceWithHttpInfo(msgVpnName, cacheName, clusterName, instanceName);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Cache Instance object. Delete a Cache Instance object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnDistributedCacheClusterInstanceWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string instanceName)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'instanceName' is set
            if (instanceName == null)
                throw new ApiException(400, "Missing required parameter 'instanceName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterInstance");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (instanceName != null) localVarPathParams.Add("instanceName", this.Configuration.ApiClient.ParameterToString(instanceName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteMsgVpnDistributedCacheClusterInstance", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Cache Instance object. Delete a Cache Instance object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnDistributedCacheClusterInstanceAsync (string msgVpnName, string cacheName, string clusterName, string instanceName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnDistributedCacheClusterInstanceAsyncWithHttpInfo(msgVpnName, cacheName, clusterName, instanceName);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Cache Instance object. Delete a Cache Instance object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnDistributedCacheClusterInstanceAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string instanceName)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'instanceName' is set
            if (instanceName == null)
                throw new ApiException(400, "Missing required parameter 'instanceName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterInstance");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (instanceName != null) localVarPathParams.Add("instanceName", this.Configuration.ApiClient.ParameterToString(instanceName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteMsgVpnDistributedCacheClusterInstance", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Topic object. Delete a Topic object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="topic">The value of the Topic in the form a/b/c.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnDistributedCacheClusterTopic (string msgVpnName, string cacheName, string clusterName, string topic)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnDistributedCacheClusterTopicWithHttpInfo(msgVpnName, cacheName, clusterName, topic);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Topic object. Delete a Topic object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="topic">The value of the Topic in the form a/b/c.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnDistributedCacheClusterTopicWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string topic)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterTopic");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterTopic");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterTopic");
            // verify the required parameter 'topic' is set
            if (topic == null)
                throw new ApiException(400, "Missing required parameter 'topic' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterTopic");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics/{topic}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (topic != null) localVarPathParams.Add("topic", this.Configuration.ApiClient.ParameterToString(topic)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteMsgVpnDistributedCacheClusterTopic", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Topic object. Delete a Topic object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="topic">The value of the Topic in the form a/b/c.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnDistributedCacheClusterTopicAsync (string msgVpnName, string cacheName, string clusterName, string topic)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnDistributedCacheClusterTopicAsyncWithHttpInfo(msgVpnName, cacheName, clusterName, topic);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Topic object. Delete a Topic object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="topic">The value of the Topic in the form a/b/c.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnDistributedCacheClusterTopicAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string topic)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterTopic");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterTopic");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterTopic");
            // verify the required parameter 'topic' is set
            if (topic == null)
                throw new ApiException(400, "Missing required parameter 'topic' when calling DistributedCacheApi->DeleteMsgVpnDistributedCacheClusterTopic");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics/{topic}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (topic != null) localVarPathParams.Add("topic", this.Configuration.ApiClient.ParameterToString(topic)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteMsgVpnDistributedCacheClusterTopic", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Get a Distributed Cache object. Get a Distributed Cache object.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheResponse</returns>
        public MsgVpnDistributedCacheResponse GetMsgVpnDistributedCache (string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheResponse> localVarResponse = GetMsgVpnDistributedCacheWithHttpInfo(msgVpnName, cacheName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Distributed Cache object. Get a Distributed Cache object.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheResponse > GetMsgVpnDistributedCacheWithHttpInfo (string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCache");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->GetMsgVpnDistributedCache");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCache", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheResponse)));
        }

        /// <summary>
        /// Get a Distributed Cache object. Get a Distributed Cache object.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheResponse> GetMsgVpnDistributedCacheAsync (string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheResponse> localVarResponse = await GetMsgVpnDistributedCacheAsyncWithHttpInfo(msgVpnName, cacheName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Distributed Cache object. Get a Distributed Cache object.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheResponse>> GetMsgVpnDistributedCacheAsyncWithHttpInfo (string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCache");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->GetMsgVpnDistributedCache");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCache", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheResponse)));
        }

        /// <summary>
        /// Get a Cache Cluster object. Get a Cache Cluster object.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterResponse</returns>
        public MsgVpnDistributedCacheClusterResponse GetMsgVpnDistributedCacheCluster (string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterResponse> localVarResponse = GetMsgVpnDistributedCacheClusterWithHttpInfo(msgVpnName, cacheName, clusterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Cache Cluster object. Get a Cache Cluster object.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheClusterResponse > GetMsgVpnDistributedCacheClusterWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheCluster");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheCluster");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheCluster");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCacheCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterResponse)));
        }

        /// <summary>
        /// Get a Cache Cluster object. Get a Cache Cluster object.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterResponse> GetMsgVpnDistributedCacheClusterAsync (string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterResponse> localVarResponse = await GetMsgVpnDistributedCacheClusterAsyncWithHttpInfo(msgVpnName, cacheName, clusterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Cache Cluster object. Get a Cache Cluster object.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterResponse>> GetMsgVpnDistributedCacheClusterAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheCluster");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheCluster");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheCluster");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCacheCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterResponse)));
        }

        /// <summary>
        /// Get a Home Cache Cluster object. Get a Home Cache Cluster object.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse</returns>
        public MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse GetMsgVpnDistributedCacheClusterGlobalCachingHomeCluster (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse> localVarResponse = GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterWithHttpInfo(msgVpnName, cacheName, clusterName, homeClusterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Home Cache Cluster object. Get a Home Cache Cluster object.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse > GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");
            // verify the required parameter 'homeClusterName' is set
            if (homeClusterName == null)
                throw new ApiException(400, "Missing required parameter 'homeClusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (homeClusterName != null) localVarPathParams.Add("homeClusterName", this.Configuration.ApiClient.ParameterToString(homeClusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCacheClusterGlobalCachingHomeCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse)));
        }

        /// <summary>
        /// Get a Home Cache Cluster object. Get a Home Cache Cluster object.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse> GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterAsync (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse> localVarResponse = await GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterAsyncWithHttpInfo(msgVpnName, cacheName, clusterName, homeClusterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Home Cache Cluster object. Get a Home Cache Cluster object.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse>> GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");
            // verify the required parameter 'homeClusterName' is set
            if (homeClusterName == null)
                throw new ApiException(400, "Missing required parameter 'homeClusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeCluster");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (homeClusterName != null) localVarPathParams.Add("homeClusterName", this.Configuration.ApiClient.ParameterToString(homeClusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCacheClusterGlobalCachingHomeCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse)));
        }

        /// <summary>
        /// Get a Topic Prefix object. Get a Topic Prefix object.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x||| topicPrefix|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="topicPrefix">A topic prefix for global topics available from the remote Home Cache Cluster. A wildcard (/&gt;) is implied at the end of the prefix.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse</returns>
        public MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string topicPrefix, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse> localVarResponse = GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixWithHttpInfo(msgVpnName, cacheName, clusterName, homeClusterName, topicPrefix, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Topic Prefix object. Get a Topic Prefix object.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x||| topicPrefix|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="topicPrefix">A topic prefix for global topics available from the remote Home Cache Cluster. A wildcard (/&gt;) is implied at the end of the prefix.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse > GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string topicPrefix, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'homeClusterName' is set
            if (homeClusterName == null)
                throw new ApiException(400, "Missing required parameter 'homeClusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'topicPrefix' is set
            if (topicPrefix == null)
                throw new ApiException(400, "Missing required parameter 'topicPrefix' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes/{topicPrefix}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (homeClusterName != null) localVarPathParams.Add("homeClusterName", this.Configuration.ApiClient.ParameterToString(homeClusterName)); // path parameter
            if (topicPrefix != null) localVarPathParams.Add("topicPrefix", this.Configuration.ApiClient.ParameterToString(topicPrefix)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse)));
        }

        /// <summary>
        /// Get a Topic Prefix object. Get a Topic Prefix object.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x||| topicPrefix|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="topicPrefix">A topic prefix for global topics available from the remote Home Cache Cluster. A wildcard (/&gt;) is implied at the end of the prefix.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse> GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixAsync (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string topicPrefix, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse> localVarResponse = await GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixAsyncWithHttpInfo(msgVpnName, cacheName, clusterName, homeClusterName, topicPrefix, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Topic Prefix object. Get a Topic Prefix object.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x||| topicPrefix|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="topicPrefix">A topic prefix for global topics available from the remote Home Cache Cluster. A wildcard (/&gt;) is implied at the end of the prefix.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse>> GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string homeClusterName, string topicPrefix, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'homeClusterName' is set
            if (homeClusterName == null)
                throw new ApiException(400, "Missing required parameter 'homeClusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");
            // verify the required parameter 'topicPrefix' is set
            if (topicPrefix == null)
                throw new ApiException(400, "Missing required parameter 'topicPrefix' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes/{topicPrefix}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (homeClusterName != null) localVarPathParams.Add("homeClusterName", this.Configuration.ApiClient.ParameterToString(homeClusterName)); // path parameter
            if (topicPrefix != null) localVarPathParams.Add("topicPrefix", this.Configuration.ApiClient.ParameterToString(topicPrefix)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse)));
        }

        /// <summary>
        /// Get a list of Topic Prefix objects. Get a list of Topic Prefix objects.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x||| topicPrefix|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse</returns>
        public MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixes (string msgVpnName, string cacheName, string clusterName, string homeClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse> localVarResponse = GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesWithHttpInfo(msgVpnName, cacheName, clusterName, homeClusterName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Topic Prefix objects. Get a list of Topic Prefix objects.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x||| topicPrefix|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse > GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string homeClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixes");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixes");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixes");
            // verify the required parameter 'homeClusterName' is set
            if (homeClusterName == null)
                throw new ApiException(400, "Missing required parameter 'homeClusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixes");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (homeClusterName != null) localVarPathParams.Add("homeClusterName", this.Configuration.ApiClient.ParameterToString(homeClusterName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixes", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse)));
        }

        /// <summary>
        /// Get a list of Topic Prefix objects. Get a list of Topic Prefix objects.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x||| topicPrefix|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse> GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesAsync (string msgVpnName, string cacheName, string clusterName, string homeClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse> localVarResponse = await GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesAsyncWithHttpInfo(msgVpnName, cacheName, clusterName, homeClusterName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Topic Prefix objects. Get a list of Topic Prefix objects.  A Topic Prefix is a prefix for a global topic that is available from the containing Home Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x||| topicPrefix|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="homeClusterName">The name of the remote Home Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse>> GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string homeClusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixes");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixes");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixes");
            // verify the required parameter 'homeClusterName' is set
            if (homeClusterName == null)
                throw new ApiException(400, "Missing required parameter 'homeClusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixes");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (homeClusterName != null) localVarPathParams.Add("homeClusterName", this.Configuration.ApiClient.ParameterToString(homeClusterName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixes", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse)));
        }

        /// <summary>
        /// Get a list of Home Cache Cluster objects. Get a list of Home Cache Cluster objects.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse</returns>
        public MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusters (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse> localVarResponse = GetMsgVpnDistributedCacheClusterGlobalCachingHomeClustersWithHttpInfo(msgVpnName, cacheName, clusterName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Home Cache Cluster objects. Get a list of Home Cache Cluster objects.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse > GetMsgVpnDistributedCacheClusterGlobalCachingHomeClustersWithHttpInfo (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusters");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusters");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusters");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusters", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse)));
        }

        /// <summary>
        /// Get a list of Home Cache Cluster objects. Get a list of Home Cache Cluster objects.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse> GetMsgVpnDistributedCacheClusterGlobalCachingHomeClustersAsync (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse> localVarResponse = await GetMsgVpnDistributedCacheClusterGlobalCachingHomeClustersAsyncWithHttpInfo(msgVpnName, cacheName, clusterName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Home Cache Cluster objects. Get a list of Home Cache Cluster objects.  A Home Cache Cluster is a Cache Cluster that is the \&quot;definitive\&quot; Cache Cluster for a given topic in the context of the Global Caching feature.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| homeClusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse>> GetMsgVpnDistributedCacheClusterGlobalCachingHomeClustersAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusters");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusters");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusters");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusters", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse)));
        }

        /// <summary>
        /// Get a Cache Instance object. Get a Cache Instance object.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| instanceName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterInstanceResponse</returns>
        public MsgVpnDistributedCacheClusterInstanceResponse GetMsgVpnDistributedCacheClusterInstance (string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse> localVarResponse = GetMsgVpnDistributedCacheClusterInstanceWithHttpInfo(msgVpnName, cacheName, clusterName, instanceName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Cache Instance object. Get a Cache Instance object.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| instanceName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterInstanceResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheClusterInstanceResponse > GetMsgVpnDistributedCacheClusterInstanceWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'instanceName' is set
            if (instanceName == null)
                throw new ApiException(400, "Missing required parameter 'instanceName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterInstance");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (instanceName != null) localVarPathParams.Add("instanceName", this.Configuration.ApiClient.ParameterToString(instanceName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCacheClusterInstance", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterInstanceResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterInstanceResponse)));
        }

        /// <summary>
        /// Get a Cache Instance object. Get a Cache Instance object.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| instanceName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterInstanceResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterInstanceResponse> GetMsgVpnDistributedCacheClusterInstanceAsync (string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse> localVarResponse = await GetMsgVpnDistributedCacheClusterInstanceAsyncWithHttpInfo(msgVpnName, cacheName, clusterName, instanceName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Cache Instance object. Get a Cache Instance object.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| instanceName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterInstanceResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse>> GetMsgVpnDistributedCacheClusterInstanceAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'instanceName' is set
            if (instanceName == null)
                throw new ApiException(400, "Missing required parameter 'instanceName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterInstance");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (instanceName != null) localVarPathParams.Add("instanceName", this.Configuration.ApiClient.ParameterToString(instanceName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCacheClusterInstance", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterInstanceResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterInstanceResponse)));
        }

        /// <summary>
        /// Get a list of Cache Instance objects. Get a list of Cache Instance objects.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| instanceName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterInstancesResponse</returns>
        public MsgVpnDistributedCacheClusterInstancesResponse GetMsgVpnDistributedCacheClusterInstances (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterInstancesResponse> localVarResponse = GetMsgVpnDistributedCacheClusterInstancesWithHttpInfo(msgVpnName, cacheName, clusterName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Cache Instance objects. Get a list of Cache Instance objects.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| instanceName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterInstancesResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheClusterInstancesResponse > GetMsgVpnDistributedCacheClusterInstancesWithHttpInfo (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterInstances");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterInstances");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterInstances");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCacheClusterInstances", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterInstancesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterInstancesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterInstancesResponse)));
        }

        /// <summary>
        /// Get a list of Cache Instance objects. Get a list of Cache Instance objects.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| instanceName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterInstancesResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterInstancesResponse> GetMsgVpnDistributedCacheClusterInstancesAsync (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterInstancesResponse> localVarResponse = await GetMsgVpnDistributedCacheClusterInstancesAsyncWithHttpInfo(msgVpnName, cacheName, clusterName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Cache Instance objects. Get a list of Cache Instance objects.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| instanceName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterInstancesResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterInstancesResponse>> GetMsgVpnDistributedCacheClusterInstancesAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterInstances");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterInstances");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterInstances");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCacheClusterInstances", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterInstancesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterInstancesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterInstancesResponse)));
        }

        /// <summary>
        /// Get a Topic object. Get a Topic object.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x||| topic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="topic">The value of the Topic in the form a/b/c.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterTopicResponse</returns>
        public MsgVpnDistributedCacheClusterTopicResponse GetMsgVpnDistributedCacheClusterTopic (string msgVpnName, string cacheName, string clusterName, string topic, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterTopicResponse> localVarResponse = GetMsgVpnDistributedCacheClusterTopicWithHttpInfo(msgVpnName, cacheName, clusterName, topic, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Topic object. Get a Topic object.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x||| topic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="topic">The value of the Topic in the form a/b/c.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterTopicResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheClusterTopicResponse > GetMsgVpnDistributedCacheClusterTopicWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string topic, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterTopic");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterTopic");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterTopic");
            // verify the required parameter 'topic' is set
            if (topic == null)
                throw new ApiException(400, "Missing required parameter 'topic' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterTopic");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics/{topic}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (topic != null) localVarPathParams.Add("topic", this.Configuration.ApiClient.ParameterToString(topic)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCacheClusterTopic", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterTopicResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterTopicResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterTopicResponse)));
        }

        /// <summary>
        /// Get a Topic object. Get a Topic object.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x||| topic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="topic">The value of the Topic in the form a/b/c.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterTopicResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterTopicResponse> GetMsgVpnDistributedCacheClusterTopicAsync (string msgVpnName, string cacheName, string clusterName, string topic, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterTopicResponse> localVarResponse = await GetMsgVpnDistributedCacheClusterTopicAsyncWithHttpInfo(msgVpnName, cacheName, clusterName, topic, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Topic object. Get a Topic object.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x||| topic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="topic">The value of the Topic in the form a/b/c.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterTopicResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterTopicResponse>> GetMsgVpnDistributedCacheClusterTopicAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, string topic, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterTopic");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterTopic");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterTopic");
            // verify the required parameter 'topic' is set
            if (topic == null)
                throw new ApiException(400, "Missing required parameter 'topic' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterTopic");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics/{topic}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (topic != null) localVarPathParams.Add("topic", this.Configuration.ApiClient.ParameterToString(topic)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCacheClusterTopic", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterTopicResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterTopicResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterTopicResponse)));
        }

        /// <summary>
        /// Get a list of Topic objects. Get a list of Topic objects.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x||| topic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterTopicsResponse</returns>
        public MsgVpnDistributedCacheClusterTopicsResponse GetMsgVpnDistributedCacheClusterTopics (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterTopicsResponse> localVarResponse = GetMsgVpnDistributedCacheClusterTopicsWithHttpInfo(msgVpnName, cacheName, clusterName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Topic objects. Get a list of Topic objects.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x||| topic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterTopicsResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheClusterTopicsResponse > GetMsgVpnDistributedCacheClusterTopicsWithHttpInfo (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterTopics");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterTopics");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterTopics");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCacheClusterTopics", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterTopicsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterTopicsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterTopicsResponse)));
        }

        /// <summary>
        /// Get a list of Topic objects. Get a list of Topic objects.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x||| topic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterTopicsResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterTopicsResponse> GetMsgVpnDistributedCacheClusterTopicsAsync (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterTopicsResponse> localVarResponse = await GetMsgVpnDistributedCacheClusterTopicsAsyncWithHttpInfo(msgVpnName, cacheName, clusterName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Topic objects. Get a list of Topic objects.  The Cache Instances that belong to the containing Cache Cluster will cache any messages published to topics that match a Topic Subscription.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x||| topic|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterTopicsResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterTopicsResponse>> GetMsgVpnDistributedCacheClusterTopicsAsyncWithHttpInfo (string msgVpnName, string cacheName, string clusterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterTopics");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterTopics");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusterTopics");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCacheClusterTopics", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterTopicsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterTopicsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterTopicsResponse)));
        }

        /// <summary>
        /// Get a list of Cache Cluster objects. Get a list of Cache Cluster objects.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClustersResponse</returns>
        public MsgVpnDistributedCacheClustersResponse GetMsgVpnDistributedCacheClusters (string msgVpnName, string cacheName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClustersResponse> localVarResponse = GetMsgVpnDistributedCacheClustersWithHttpInfo(msgVpnName, cacheName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Cache Cluster objects. Get a list of Cache Cluster objects.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClustersResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheClustersResponse > GetMsgVpnDistributedCacheClustersWithHttpInfo (string msgVpnName, string cacheName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusters");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusters");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCacheClusters", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClustersResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClustersResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClustersResponse)));
        }

        /// <summary>
        /// Get a list of Cache Cluster objects. Get a list of Cache Cluster objects.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClustersResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheClustersResponse> GetMsgVpnDistributedCacheClustersAsync (string msgVpnName, string cacheName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClustersResponse> localVarResponse = await GetMsgVpnDistributedCacheClustersAsyncWithHttpInfo(msgVpnName, cacheName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Cache Cluster objects. Get a list of Cache Cluster objects.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| clusterName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClustersResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClustersResponse>> GetMsgVpnDistributedCacheClustersAsyncWithHttpInfo (string msgVpnName, string cacheName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusters");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->GetMsgVpnDistributedCacheClusters");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCacheClusters", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClustersResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClustersResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClustersResponse)));
        }

        /// <summary>
        /// Get a list of Distributed Cache objects. Get a list of Distributed Cache objects.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCachesResponse</returns>
        public MsgVpnDistributedCachesResponse GetMsgVpnDistributedCaches (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCachesResponse> localVarResponse = GetMsgVpnDistributedCachesWithHttpInfo(msgVpnName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Distributed Cache objects. Get a list of Distributed Cache objects.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCachesResponse</returns>
        public ApiResponse< MsgVpnDistributedCachesResponse > GetMsgVpnDistributedCachesWithHttpInfo (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCaches");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCaches", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCachesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCachesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCachesResponse)));
        }

        /// <summary>
        /// Get a list of Distributed Cache objects. Get a list of Distributed Cache objects.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCachesResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCachesResponse> GetMsgVpnDistributedCachesAsync (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCachesResponse> localVarResponse = await GetMsgVpnDistributedCachesAsyncWithHttpInfo(msgVpnName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Distributed Cache objects. Get a list of Distributed Cache objects.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: cacheName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCachesResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCachesResponse>> GetMsgVpnDistributedCachesAsyncWithHttpInfo (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->GetMsgVpnDistributedCaches");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetMsgVpnDistributedCaches", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCachesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCachesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCachesResponse)));
        }

        /// <summary>
        /// Replace a Distributed Cache object. Replace a Distributed Cache object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| cacheVirtualRouter||x|||||| msgVpnName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheResponse</returns>
        public MsgVpnDistributedCacheResponse ReplaceMsgVpnDistributedCache (MsgVpnDistributedCache body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheResponse> localVarResponse = ReplaceMsgVpnDistributedCacheWithHttpInfo(body, msgVpnName, cacheName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Replace a Distributed Cache object. Replace a Distributed Cache object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| cacheVirtualRouter||x|||||| msgVpnName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheResponse > ReplaceMsgVpnDistributedCacheWithHttpInfo (MsgVpnDistributedCache body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCache");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCache");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCache");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceMsgVpnDistributedCache", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheResponse)));
        }

        /// <summary>
        /// Replace a Distributed Cache object. Replace a Distributed Cache object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| cacheVirtualRouter||x|||||| msgVpnName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheResponse> ReplaceMsgVpnDistributedCacheAsync (MsgVpnDistributedCache body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheResponse> localVarResponse = await ReplaceMsgVpnDistributedCacheAsyncWithHttpInfo(body, msgVpnName, cacheName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Replace a Distributed Cache object. Replace a Distributed Cache object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| cacheVirtualRouter||x|||||| msgVpnName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheResponse>> ReplaceMsgVpnDistributedCacheAsyncWithHttpInfo (MsgVpnDistributedCache body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCache");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCache");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCache");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceMsgVpnDistributedCache", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheResponse)));
        }

        /// <summary>
        /// Replace a Cache Cluster object. Replace a Cache Cluster object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| clusterName|x||x||||| deliverToOneOverrideEnabled||||||x|| msgVpnName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterResponse</returns>
        public MsgVpnDistributedCacheClusterResponse ReplaceMsgVpnDistributedCacheCluster (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterResponse> localVarResponse = ReplaceMsgVpnDistributedCacheClusterWithHttpInfo(body, msgVpnName, cacheName, clusterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Replace a Cache Cluster object. Replace a Cache Cluster object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| clusterName|x||x||||| deliverToOneOverrideEnabled||||||x|| msgVpnName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheClusterResponse > ReplaceMsgVpnDistributedCacheClusterWithHttpInfo (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCacheCluster");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCacheCluster");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCacheCluster");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCacheCluster");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceMsgVpnDistributedCacheCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterResponse)));
        }

        /// <summary>
        /// Replace a Cache Cluster object. Replace a Cache Cluster object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| clusterName|x||x||||| deliverToOneOverrideEnabled||||||x|| msgVpnName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterResponse> ReplaceMsgVpnDistributedCacheClusterAsync (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterResponse> localVarResponse = await ReplaceMsgVpnDistributedCacheClusterAsyncWithHttpInfo(body, msgVpnName, cacheName, clusterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Replace a Cache Cluster object. Replace a Cache Cluster object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| clusterName|x||x||||| deliverToOneOverrideEnabled||||||x|| msgVpnName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterResponse>> ReplaceMsgVpnDistributedCacheClusterAsyncWithHttpInfo (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCacheCluster");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCacheCluster");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCacheCluster");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCacheCluster");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceMsgVpnDistributedCacheCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterResponse)));
        }

        /// <summary>
        /// Replace a Cache Instance object. Replace a Cache Instance object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| clusterName|x||x||||| instanceName|x||x||||| msgVpnName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterInstanceResponse</returns>
        public MsgVpnDistributedCacheClusterInstanceResponse ReplaceMsgVpnDistributedCacheClusterInstance (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse> localVarResponse = ReplaceMsgVpnDistributedCacheClusterInstanceWithHttpInfo(body, msgVpnName, cacheName, clusterName, instanceName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Replace a Cache Instance object. Replace a Cache Instance object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| clusterName|x||x||||| instanceName|x||x||||| msgVpnName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterInstanceResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheClusterInstanceResponse > ReplaceMsgVpnDistributedCacheClusterInstanceWithHttpInfo (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'instanceName' is set
            if (instanceName == null)
                throw new ApiException(400, "Missing required parameter 'instanceName' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCacheClusterInstance");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (instanceName != null) localVarPathParams.Add("instanceName", this.Configuration.ApiClient.ParameterToString(instanceName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceMsgVpnDistributedCacheClusterInstance", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterInstanceResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterInstanceResponse)));
        }

        /// <summary>
        /// Replace a Cache Instance object. Replace a Cache Instance object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| clusterName|x||x||||| instanceName|x||x||||| msgVpnName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterInstanceResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterInstanceResponse> ReplaceMsgVpnDistributedCacheClusterInstanceAsync (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse> localVarResponse = await ReplaceMsgVpnDistributedCacheClusterInstanceAsyncWithHttpInfo(body, msgVpnName, cacheName, clusterName, instanceName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Replace a Cache Instance object. Replace a Cache Instance object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x||x||||| clusterName|x||x||||| instanceName|x||x||||| msgVpnName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterInstanceResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse>> ReplaceMsgVpnDistributedCacheClusterInstanceAsyncWithHttpInfo (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'instanceName' is set
            if (instanceName == null)
                throw new ApiException(400, "Missing required parameter 'instanceName' when calling DistributedCacheApi->ReplaceMsgVpnDistributedCacheClusterInstance");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (instanceName != null) localVarPathParams.Add("instanceName", this.Configuration.ApiClient.ParameterToString(instanceName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceMsgVpnDistributedCacheClusterInstance", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterInstanceResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterInstanceResponse)));
        }

        /// <summary>
        /// Update a Distributed Cache object. Update a Distributed Cache object. Any attribute missing from the request will be left unchanged.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| cacheVirtualRouter||x||||| msgVpnName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheResponse</returns>
        public MsgVpnDistributedCacheResponse UpdateMsgVpnDistributedCache (MsgVpnDistributedCache body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheResponse> localVarResponse = UpdateMsgVpnDistributedCacheWithHttpInfo(body, msgVpnName, cacheName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Update a Distributed Cache object. Update a Distributed Cache object. Any attribute missing from the request will be left unchanged.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| cacheVirtualRouter||x||||| msgVpnName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheResponse > UpdateMsgVpnDistributedCacheWithHttpInfo (MsgVpnDistributedCache body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->UpdateMsgVpnDistributedCache");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->UpdateMsgVpnDistributedCache");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->UpdateMsgVpnDistributedCache");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateMsgVpnDistributedCache", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheResponse)));
        }

        /// <summary>
        /// Update a Distributed Cache object. Update a Distributed Cache object. Any attribute missing from the request will be left unchanged.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| cacheVirtualRouter||x||||| msgVpnName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheResponse> UpdateMsgVpnDistributedCacheAsync (MsgVpnDistributedCache body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheResponse> localVarResponse = await UpdateMsgVpnDistributedCacheAsyncWithHttpInfo(body, msgVpnName, cacheName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Update a Distributed Cache object. Update a Distributed Cache object. Any attribute missing from the request will be left unchanged.  A Distributed Cache is a collection of one or more Cache Clusters that belong to the same Message VPN. Each Cache Cluster in a Distributed Cache is configured to subscribe to a different set of topics. This effectively divides up the configured topic space, to provide scaling to very large topic spaces or very high cached message throughput.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| cacheVirtualRouter||x||||| msgVpnName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnDistributedCache|scheduledDeleteMsgDayList|scheduledDeleteMsgTimeList| MsgVpnDistributedCache|scheduledDeleteMsgTimeList|scheduledDeleteMsgDayList|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Distributed Cache object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheResponse>> UpdateMsgVpnDistributedCacheAsyncWithHttpInfo (MsgVpnDistributedCache body, string msgVpnName, string cacheName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->UpdateMsgVpnDistributedCache");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->UpdateMsgVpnDistributedCache");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->UpdateMsgVpnDistributedCache");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateMsgVpnDistributedCache", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheResponse)));
        }

        /// <summary>
        /// Update a Cache Cluster object. Update a Cache Cluster object. Any attribute missing from the request will be left unchanged.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| clusterName|x|x||||| deliverToOneOverrideEnabled|||||x|| msgVpnName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterResponse</returns>
        public MsgVpnDistributedCacheClusterResponse UpdateMsgVpnDistributedCacheCluster (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterResponse> localVarResponse = UpdateMsgVpnDistributedCacheClusterWithHttpInfo(body, msgVpnName, cacheName, clusterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Update a Cache Cluster object. Update a Cache Cluster object. Any attribute missing from the request will be left unchanged.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| clusterName|x|x||||| deliverToOneOverrideEnabled|||||x|| msgVpnName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheClusterResponse > UpdateMsgVpnDistributedCacheClusterWithHttpInfo (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->UpdateMsgVpnDistributedCacheCluster");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->UpdateMsgVpnDistributedCacheCluster");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->UpdateMsgVpnDistributedCacheCluster");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->UpdateMsgVpnDistributedCacheCluster");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateMsgVpnDistributedCacheCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterResponse)));
        }

        /// <summary>
        /// Update a Cache Cluster object. Update a Cache Cluster object. Any attribute missing from the request will be left unchanged.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| clusterName|x|x||||| deliverToOneOverrideEnabled|||||x|| msgVpnName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterResponse> UpdateMsgVpnDistributedCacheClusterAsync (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterResponse> localVarResponse = await UpdateMsgVpnDistributedCacheClusterAsyncWithHttpInfo(body, msgVpnName, cacheName, clusterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Update a Cache Cluster object. Update a Cache Cluster object. Any attribute missing from the request will be left unchanged.  A Cache Cluster is a collection of one or more Cache Instances that subscribe to exactly the same topics. Cache Instances are grouped together in a Cache Cluster for the purpose of fault tolerance and load balancing. As published messages are received, the message broker message bus sends these live data messages to the Cache Instances in the Cache Cluster. This enables client cache requests to be served by any of Cache Instances in the Cache Cluster.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| clusterName|x|x||||| deliverToOneOverrideEnabled|||||x|| msgVpnName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThresholdByPercent|clearPercent|setPercent| EventThresholdByPercent|setPercent|clearPercent| EventThresholdByValue|clearValue|setValue| EventThresholdByValue|setValue|clearValue|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Cluster object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterResponse>> UpdateMsgVpnDistributedCacheClusterAsyncWithHttpInfo (MsgVpnDistributedCacheCluster body, string msgVpnName, string cacheName, string clusterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->UpdateMsgVpnDistributedCacheCluster");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->UpdateMsgVpnDistributedCacheCluster");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->UpdateMsgVpnDistributedCacheCluster");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->UpdateMsgVpnDistributedCacheCluster");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateMsgVpnDistributedCacheCluster", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterResponse)));
        }

        /// <summary>
        /// Update a Cache Instance object. Update a Cache Instance object. Any attribute missing from the request will be left unchanged.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| clusterName|x|x||||| instanceName|x|x||||| msgVpnName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnDistributedCacheClusterInstanceResponse</returns>
        public MsgVpnDistributedCacheClusterInstanceResponse UpdateMsgVpnDistributedCacheClusterInstance (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse> localVarResponse = UpdateMsgVpnDistributedCacheClusterInstanceWithHttpInfo(body, msgVpnName, cacheName, clusterName, instanceName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Update a Cache Instance object. Update a Cache Instance object. Any attribute missing from the request will be left unchanged.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| clusterName|x|x||||| instanceName|x|x||||| msgVpnName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnDistributedCacheClusterInstanceResponse</returns>
        public ApiResponse< MsgVpnDistributedCacheClusterInstanceResponse > UpdateMsgVpnDistributedCacheClusterInstanceWithHttpInfo (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->UpdateMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->UpdateMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->UpdateMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->UpdateMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'instanceName' is set
            if (instanceName == null)
                throw new ApiException(400, "Missing required parameter 'instanceName' when calling DistributedCacheApi->UpdateMsgVpnDistributedCacheClusterInstance");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (instanceName != null) localVarPathParams.Add("instanceName", this.Configuration.ApiClient.ParameterToString(instanceName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateMsgVpnDistributedCacheClusterInstance", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterInstanceResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterInstanceResponse)));
        }

        /// <summary>
        /// Update a Cache Instance object. Update a Cache Instance object. Any attribute missing from the request will be left unchanged.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| clusterName|x|x||||| instanceName|x|x||||| msgVpnName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnDistributedCacheClusterInstanceResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnDistributedCacheClusterInstanceResponse> UpdateMsgVpnDistributedCacheClusterInstanceAsync (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse> localVarResponse = await UpdateMsgVpnDistributedCacheClusterInstanceAsyncWithHttpInfo(body, msgVpnName, cacheName, clusterName, instanceName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Update a Cache Instance object. Update a Cache Instance object. Any attribute missing from the request will be left unchanged.  A Cache Instance is a single Cache process that belongs to a single Cache Cluster. A Cache Instance object provisioned on the broker is used to disseminate configuration information to the Cache process. Cache Instances listen for and cache live data messages that match the topic subscriptions configured for their parent Cache Cluster.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- cacheName|x|x||||| clusterName|x|x||||| instanceName|x|x||||| msgVpnName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.11.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Cache Instance object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="cacheName">The name of the Distributed Cache.</param>
        /// <param name="clusterName">The name of the Cache Cluster.</param>
        /// <param name="instanceName">The name of the Cache Instance.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnDistributedCacheClusterInstanceResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse>> UpdateMsgVpnDistributedCacheClusterInstanceAsyncWithHttpInfo (MsgVpnDistributedCacheClusterInstance body, string msgVpnName, string cacheName, string clusterName, string instanceName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling DistributedCacheApi->UpdateMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling DistributedCacheApi->UpdateMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'cacheName' is set
            if (cacheName == null)
                throw new ApiException(400, "Missing required parameter 'cacheName' when calling DistributedCacheApi->UpdateMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'clusterName' is set
            if (clusterName == null)
                throw new ApiException(400, "Missing required parameter 'clusterName' when calling DistributedCacheApi->UpdateMsgVpnDistributedCacheClusterInstance");
            // verify the required parameter 'instanceName' is set
            if (instanceName == null)
                throw new ApiException(400, "Missing required parameter 'instanceName' when calling DistributedCacheApi->UpdateMsgVpnDistributedCacheClusterInstance");

            var localVarPath = "./msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (cacheName != null) localVarPathParams.Add("cacheName", this.Configuration.ApiClient.ParameterToString(cacheName)); // path parameter
            if (clusterName != null) localVarPathParams.Add("clusterName", this.Configuration.ApiClient.ParameterToString(clusterName)); // path parameter
            if (instanceName != null) localVarPathParams.Add("instanceName", this.Configuration.ApiClient.ParameterToString(instanceName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateMsgVpnDistributedCacheClusterInstance", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnDistributedCacheClusterInstanceResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnDistributedCacheClusterInstanceResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnDistributedCacheClusterInstanceResponse)));
        }

    }
}
