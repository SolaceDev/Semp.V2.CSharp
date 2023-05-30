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
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SwaggerDateConverter = Semp.V2.CSharp.Client.SwaggerDateConverter;

namespace Semp.V2.CSharp.Model
{
    /// <summary>
    /// MsgVpnDistributedCacheCluster
    /// </summary>
    [DataContract]
        public partial class MsgVpnDistributedCacheCluster :  IEquatable<MsgVpnDistributedCacheCluster>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MsgVpnDistributedCacheCluster" /> class.
        /// </summary>
        /// <param name="cacheName">The name of the Distributed Cache..</param>
        /// <param name="clusterName">The name of the Cache Cluster..</param>
        /// <param name="deliverToOneOverrideEnabled">Enable or disable deliver-to-one override for the Cache Cluster. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="enabled">Enable or disable the Cache Cluster. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="eventDataByteRateThreshold">eventDataByteRateThreshold.</param>
        /// <param name="eventDataMsgRateThreshold">eventDataMsgRateThreshold.</param>
        /// <param name="eventMaxMemoryThreshold">eventMaxMemoryThreshold.</param>
        /// <param name="eventMaxTopicsThreshold">eventMaxTopicsThreshold.</param>
        /// <param name="eventRequestQueueDepthThreshold">eventRequestQueueDepthThreshold.</param>
        /// <param name="eventRequestRateThreshold">eventRequestRateThreshold.</param>
        /// <param name="eventResponseRateThreshold">eventResponseRateThreshold.</param>
        /// <param name="globalCachingEnabled">Enable or disable global caching for the Cache Cluster. When enabled, the Cache Instances will fetch topics from remote Home Cache Clusters when requested, and subscribe to those topics to cache them locally. When disabled, the Cache Instances will remove all subscriptions and cached messages for topics from remote Home Cache Clusters. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="globalCachingHeartbeat">The heartbeat interval, in seconds, used by the Cache Instances to monitor connectivity with the remote Home Cache Clusters. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;..</param>
        /// <param name="globalCachingTopicLifetime">The topic lifetime, in seconds. If no client requests are received for a given global topic over the duration of the topic lifetime, then the Cache Instance will remove the subscription and cached messages for that topic. A value of 0 disables aging. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3600&#x60;..</param>
        /// <param name="maxMemory">The maximum memory usage, in megabytes (MB), for each Cache Instance in the Cache Cluster. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;2048&#x60;..</param>
        /// <param name="maxMsgsPerTopic">The maximum number of messages per topic for each Cache Instance in the Cache Cluster. When at the maximum, old messages are removed as new messages arrive. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1&#x60;..</param>
        /// <param name="maxRequestQueueDepth">The maximum queue depth for cache requests received by the Cache Cluster. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;100000&#x60;..</param>
        /// <param name="maxTopicCount">The maximum number of topics for each Cache Instance in the Cache Cluster. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;2000000&#x60;..</param>
        /// <param name="msgLifetime">The message lifetime, in seconds. If a message remains cached for the duration of its lifetime, the Cache Instance will remove the message. A lifetime of 0 results in the message being retained indefinitely. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;..</param>
        /// <param name="msgVpnName">The name of the Message VPN..</param>
        /// <param name="newTopicAdvertisementEnabled">Enable or disable the advertising, onto the message bus, of new topics learned by each Cache Instance in the Cache Cluster. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        public MsgVpnDistributedCacheCluster(string cacheName = default(string), string clusterName = default(string), bool? deliverToOneOverrideEnabled = default(bool?), bool? enabled = default(bool?), EventThresholdByValue eventDataByteRateThreshold = default(EventThresholdByValue), EventThresholdByValue eventDataMsgRateThreshold = default(EventThresholdByValue), EventThresholdByPercent eventMaxMemoryThreshold = default(EventThresholdByPercent), EventThresholdByPercent eventMaxTopicsThreshold = default(EventThresholdByPercent), EventThresholdByPercent eventRequestQueueDepthThreshold = default(EventThresholdByPercent), EventThresholdByValue eventRequestRateThreshold = default(EventThresholdByValue), EventThresholdByValue eventResponseRateThreshold = default(EventThresholdByValue), bool? globalCachingEnabled = default(bool?), long? globalCachingHeartbeat = default(long?), long? globalCachingTopicLifetime = default(long?), long? maxMemory = default(long?), long? maxMsgsPerTopic = default(long?), long? maxRequestQueueDepth = default(long?), long? maxTopicCount = default(long?), long? msgLifetime = default(long?), string msgVpnName = default(string), bool? newTopicAdvertisementEnabled = default(bool?))
        {
            this.CacheName = cacheName;
            this.ClusterName = clusterName;
            this.DeliverToOneOverrideEnabled = deliverToOneOverrideEnabled;
            this.Enabled = enabled;
            this.EventDataByteRateThreshold = eventDataByteRateThreshold;
            this.EventDataMsgRateThreshold = eventDataMsgRateThreshold;
            this.EventMaxMemoryThreshold = eventMaxMemoryThreshold;
            this.EventMaxTopicsThreshold = eventMaxTopicsThreshold;
            this.EventRequestQueueDepthThreshold = eventRequestQueueDepthThreshold;
            this.EventRequestRateThreshold = eventRequestRateThreshold;
            this.EventResponseRateThreshold = eventResponseRateThreshold;
            this.GlobalCachingEnabled = globalCachingEnabled;
            this.GlobalCachingHeartbeat = globalCachingHeartbeat;
            this.GlobalCachingTopicLifetime = globalCachingTopicLifetime;
            this.MaxMemory = maxMemory;
            this.MaxMsgsPerTopic = maxMsgsPerTopic;
            this.MaxRequestQueueDepth = maxRequestQueueDepth;
            this.MaxTopicCount = maxTopicCount;
            this.MsgLifetime = msgLifetime;
            this.MsgVpnName = msgVpnName;
            this.NewTopicAdvertisementEnabled = newTopicAdvertisementEnabled;
        }
        
        /// <summary>
        /// The name of the Distributed Cache.
        /// </summary>
        /// <value>The name of the Distributed Cache.</value>
        [DataMember(Name="cacheName", EmitDefaultValue=false)]
        public string CacheName { get; set; }

        /// <summary>
        /// The name of the Cache Cluster.
        /// </summary>
        /// <value>The name of the Cache Cluster.</value>
        [DataMember(Name="clusterName", EmitDefaultValue=false)]
        public string ClusterName { get; set; }

        /// <summary>
        /// Enable or disable deliver-to-one override for the Cache Cluster. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable deliver-to-one override for the Cache Cluster. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="deliverToOneOverrideEnabled", EmitDefaultValue=false)]
        public bool? DeliverToOneOverrideEnabled { get; set; }

        /// <summary>
        /// Enable or disable the Cache Cluster. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable the Cache Cluster. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="enabled", EmitDefaultValue=false)]
        public bool? Enabled { get; set; }

        /// <summary>
        /// Gets or Sets EventDataByteRateThreshold
        /// </summary>
        [DataMember(Name="eventDataByteRateThreshold", EmitDefaultValue=false)]
        public EventThresholdByValue EventDataByteRateThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventDataMsgRateThreshold
        /// </summary>
        [DataMember(Name="eventDataMsgRateThreshold", EmitDefaultValue=false)]
        public EventThresholdByValue EventDataMsgRateThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventMaxMemoryThreshold
        /// </summary>
        [DataMember(Name="eventMaxMemoryThreshold", EmitDefaultValue=false)]
        public EventThresholdByPercent EventMaxMemoryThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventMaxTopicsThreshold
        /// </summary>
        [DataMember(Name="eventMaxTopicsThreshold", EmitDefaultValue=false)]
        public EventThresholdByPercent EventMaxTopicsThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventRequestQueueDepthThreshold
        /// </summary>
        [DataMember(Name="eventRequestQueueDepthThreshold", EmitDefaultValue=false)]
        public EventThresholdByPercent EventRequestQueueDepthThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventRequestRateThreshold
        /// </summary>
        [DataMember(Name="eventRequestRateThreshold", EmitDefaultValue=false)]
        public EventThresholdByValue EventRequestRateThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventResponseRateThreshold
        /// </summary>
        [DataMember(Name="eventResponseRateThreshold", EmitDefaultValue=false)]
        public EventThresholdByValue EventResponseRateThreshold { get; set; }

        /// <summary>
        /// Enable or disable global caching for the Cache Cluster. When enabled, the Cache Instances will fetch topics from remote Home Cache Clusters when requested, and subscribe to those topics to cache them locally. When disabled, the Cache Instances will remove all subscriptions and cached messages for topics from remote Home Cache Clusters. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable global caching for the Cache Cluster. When enabled, the Cache Instances will fetch topics from remote Home Cache Clusters when requested, and subscribe to those topics to cache them locally. When disabled, the Cache Instances will remove all subscriptions and cached messages for topics from remote Home Cache Clusters. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="globalCachingEnabled", EmitDefaultValue=false)]
        public bool? GlobalCachingEnabled { get; set; }

        /// <summary>
        /// The heartbeat interval, in seconds, used by the Cache Instances to monitor connectivity with the remote Home Cache Clusters. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;.
        /// </summary>
        /// <value>The heartbeat interval, in seconds, used by the Cache Instances to monitor connectivity with the remote Home Cache Clusters. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;.</value>
        [DataMember(Name="globalCachingHeartbeat", EmitDefaultValue=false)]
        public long? GlobalCachingHeartbeat { get; set; }

        /// <summary>
        /// The topic lifetime, in seconds. If no client requests are received for a given global topic over the duration of the topic lifetime, then the Cache Instance will remove the subscription and cached messages for that topic. A value of 0 disables aging. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3600&#x60;.
        /// </summary>
        /// <value>The topic lifetime, in seconds. If no client requests are received for a given global topic over the duration of the topic lifetime, then the Cache Instance will remove the subscription and cached messages for that topic. A value of 0 disables aging. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3600&#x60;.</value>
        [DataMember(Name="globalCachingTopicLifetime", EmitDefaultValue=false)]
        public long? GlobalCachingTopicLifetime { get; set; }

        /// <summary>
        /// The maximum memory usage, in megabytes (MB), for each Cache Instance in the Cache Cluster. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;2048&#x60;.
        /// </summary>
        /// <value>The maximum memory usage, in megabytes (MB), for each Cache Instance in the Cache Cluster. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;2048&#x60;.</value>
        [DataMember(Name="maxMemory", EmitDefaultValue=false)]
        public long? MaxMemory { get; set; }

        /// <summary>
        /// The maximum number of messages per topic for each Cache Instance in the Cache Cluster. When at the maximum, old messages are removed as new messages arrive. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1&#x60;.
        /// </summary>
        /// <value>The maximum number of messages per topic for each Cache Instance in the Cache Cluster. When at the maximum, old messages are removed as new messages arrive. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1&#x60;.</value>
        [DataMember(Name="maxMsgsPerTopic", EmitDefaultValue=false)]
        public long? MaxMsgsPerTopic { get; set; }

        /// <summary>
        /// The maximum queue depth for cache requests received by the Cache Cluster. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;100000&#x60;.
        /// </summary>
        /// <value>The maximum queue depth for cache requests received by the Cache Cluster. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;100000&#x60;.</value>
        [DataMember(Name="maxRequestQueueDepth", EmitDefaultValue=false)]
        public long? MaxRequestQueueDepth { get; set; }

        /// <summary>
        /// The maximum number of topics for each Cache Instance in the Cache Cluster. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;2000000&#x60;.
        /// </summary>
        /// <value>The maximum number of topics for each Cache Instance in the Cache Cluster. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;2000000&#x60;.</value>
        [DataMember(Name="maxTopicCount", EmitDefaultValue=false)]
        public long? MaxTopicCount { get; set; }

        /// <summary>
        /// The message lifetime, in seconds. If a message remains cached for the duration of its lifetime, the Cache Instance will remove the message. A lifetime of 0 results in the message being retained indefinitely. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;.
        /// </summary>
        /// <value>The message lifetime, in seconds. If a message remains cached for the duration of its lifetime, the Cache Instance will remove the message. A lifetime of 0 results in the message being retained indefinitely. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;.</value>
        [DataMember(Name="msgLifetime", EmitDefaultValue=false)]
        public long? MsgLifetime { get; set; }

        /// <summary>
        /// The name of the Message VPN.
        /// </summary>
        /// <value>The name of the Message VPN.</value>
        [DataMember(Name="msgVpnName", EmitDefaultValue=false)]
        public string MsgVpnName { get; set; }

        /// <summary>
        /// Enable or disable the advertising, onto the message bus, of new topics learned by each Cache Instance in the Cache Cluster. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable the advertising, onto the message bus, of new topics learned by each Cache Instance in the Cache Cluster. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="newTopicAdvertisementEnabled", EmitDefaultValue=false)]
        public bool? NewTopicAdvertisementEnabled { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class MsgVpnDistributedCacheCluster {\n");
            sb.Append("  CacheName: ").Append(CacheName).Append("\n");
            sb.Append("  ClusterName: ").Append(ClusterName).Append("\n");
            sb.Append("  DeliverToOneOverrideEnabled: ").Append(DeliverToOneOverrideEnabled).Append("\n");
            sb.Append("  Enabled: ").Append(Enabled).Append("\n");
            sb.Append("  EventDataByteRateThreshold: ").Append(EventDataByteRateThreshold).Append("\n");
            sb.Append("  EventDataMsgRateThreshold: ").Append(EventDataMsgRateThreshold).Append("\n");
            sb.Append("  EventMaxMemoryThreshold: ").Append(EventMaxMemoryThreshold).Append("\n");
            sb.Append("  EventMaxTopicsThreshold: ").Append(EventMaxTopicsThreshold).Append("\n");
            sb.Append("  EventRequestQueueDepthThreshold: ").Append(EventRequestQueueDepthThreshold).Append("\n");
            sb.Append("  EventRequestRateThreshold: ").Append(EventRequestRateThreshold).Append("\n");
            sb.Append("  EventResponseRateThreshold: ").Append(EventResponseRateThreshold).Append("\n");
            sb.Append("  GlobalCachingEnabled: ").Append(GlobalCachingEnabled).Append("\n");
            sb.Append("  GlobalCachingHeartbeat: ").Append(GlobalCachingHeartbeat).Append("\n");
            sb.Append("  GlobalCachingTopicLifetime: ").Append(GlobalCachingTopicLifetime).Append("\n");
            sb.Append("  MaxMemory: ").Append(MaxMemory).Append("\n");
            sb.Append("  MaxMsgsPerTopic: ").Append(MaxMsgsPerTopic).Append("\n");
            sb.Append("  MaxRequestQueueDepth: ").Append(MaxRequestQueueDepth).Append("\n");
            sb.Append("  MaxTopicCount: ").Append(MaxTopicCount).Append("\n");
            sb.Append("  MsgLifetime: ").Append(MsgLifetime).Append("\n");
            sb.Append("  MsgVpnName: ").Append(MsgVpnName).Append("\n");
            sb.Append("  NewTopicAdvertisementEnabled: ").Append(NewTopicAdvertisementEnabled).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as MsgVpnDistributedCacheCluster);
        }

        /// <summary>
        /// Returns true if MsgVpnDistributedCacheCluster instances are equal
        /// </summary>
        /// <param name="input">Instance of MsgVpnDistributedCacheCluster to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(MsgVpnDistributedCacheCluster input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.CacheName == input.CacheName ||
                    (this.CacheName != null &&
                    this.CacheName.Equals(input.CacheName))
                ) && 
                (
                    this.ClusterName == input.ClusterName ||
                    (this.ClusterName != null &&
                    this.ClusterName.Equals(input.ClusterName))
                ) && 
                (
                    this.DeliverToOneOverrideEnabled == input.DeliverToOneOverrideEnabled ||
                    (this.DeliverToOneOverrideEnabled != null &&
                    this.DeliverToOneOverrideEnabled.Equals(input.DeliverToOneOverrideEnabled))
                ) && 
                (
                    this.Enabled == input.Enabled ||
                    (this.Enabled != null &&
                    this.Enabled.Equals(input.Enabled))
                ) && 
                (
                    this.EventDataByteRateThreshold == input.EventDataByteRateThreshold ||
                    (this.EventDataByteRateThreshold != null &&
                    this.EventDataByteRateThreshold.Equals(input.EventDataByteRateThreshold))
                ) && 
                (
                    this.EventDataMsgRateThreshold == input.EventDataMsgRateThreshold ||
                    (this.EventDataMsgRateThreshold != null &&
                    this.EventDataMsgRateThreshold.Equals(input.EventDataMsgRateThreshold))
                ) && 
                (
                    this.EventMaxMemoryThreshold == input.EventMaxMemoryThreshold ||
                    (this.EventMaxMemoryThreshold != null &&
                    this.EventMaxMemoryThreshold.Equals(input.EventMaxMemoryThreshold))
                ) && 
                (
                    this.EventMaxTopicsThreshold == input.EventMaxTopicsThreshold ||
                    (this.EventMaxTopicsThreshold != null &&
                    this.EventMaxTopicsThreshold.Equals(input.EventMaxTopicsThreshold))
                ) && 
                (
                    this.EventRequestQueueDepthThreshold == input.EventRequestQueueDepthThreshold ||
                    (this.EventRequestQueueDepthThreshold != null &&
                    this.EventRequestQueueDepthThreshold.Equals(input.EventRequestQueueDepthThreshold))
                ) && 
                (
                    this.EventRequestRateThreshold == input.EventRequestRateThreshold ||
                    (this.EventRequestRateThreshold != null &&
                    this.EventRequestRateThreshold.Equals(input.EventRequestRateThreshold))
                ) && 
                (
                    this.EventResponseRateThreshold == input.EventResponseRateThreshold ||
                    (this.EventResponseRateThreshold != null &&
                    this.EventResponseRateThreshold.Equals(input.EventResponseRateThreshold))
                ) && 
                (
                    this.GlobalCachingEnabled == input.GlobalCachingEnabled ||
                    (this.GlobalCachingEnabled != null &&
                    this.GlobalCachingEnabled.Equals(input.GlobalCachingEnabled))
                ) && 
                (
                    this.GlobalCachingHeartbeat == input.GlobalCachingHeartbeat ||
                    (this.GlobalCachingHeartbeat != null &&
                    this.GlobalCachingHeartbeat.Equals(input.GlobalCachingHeartbeat))
                ) && 
                (
                    this.GlobalCachingTopicLifetime == input.GlobalCachingTopicLifetime ||
                    (this.GlobalCachingTopicLifetime != null &&
                    this.GlobalCachingTopicLifetime.Equals(input.GlobalCachingTopicLifetime))
                ) && 
                (
                    this.MaxMemory == input.MaxMemory ||
                    (this.MaxMemory != null &&
                    this.MaxMemory.Equals(input.MaxMemory))
                ) && 
                (
                    this.MaxMsgsPerTopic == input.MaxMsgsPerTopic ||
                    (this.MaxMsgsPerTopic != null &&
                    this.MaxMsgsPerTopic.Equals(input.MaxMsgsPerTopic))
                ) && 
                (
                    this.MaxRequestQueueDepth == input.MaxRequestQueueDepth ||
                    (this.MaxRequestQueueDepth != null &&
                    this.MaxRequestQueueDepth.Equals(input.MaxRequestQueueDepth))
                ) && 
                (
                    this.MaxTopicCount == input.MaxTopicCount ||
                    (this.MaxTopicCount != null &&
                    this.MaxTopicCount.Equals(input.MaxTopicCount))
                ) && 
                (
                    this.MsgLifetime == input.MsgLifetime ||
                    (this.MsgLifetime != null &&
                    this.MsgLifetime.Equals(input.MsgLifetime))
                ) && 
                (
                    this.MsgVpnName == input.MsgVpnName ||
                    (this.MsgVpnName != null &&
                    this.MsgVpnName.Equals(input.MsgVpnName))
                ) && 
                (
                    this.NewTopicAdvertisementEnabled == input.NewTopicAdvertisementEnabled ||
                    (this.NewTopicAdvertisementEnabled != null &&
                    this.NewTopicAdvertisementEnabled.Equals(input.NewTopicAdvertisementEnabled))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.CacheName != null)
                    hashCode = hashCode * 59 + this.CacheName.GetHashCode();
                if (this.ClusterName != null)
                    hashCode = hashCode * 59 + this.ClusterName.GetHashCode();
                if (this.DeliverToOneOverrideEnabled != null)
                    hashCode = hashCode * 59 + this.DeliverToOneOverrideEnabled.GetHashCode();
                if (this.Enabled != null)
                    hashCode = hashCode * 59 + this.Enabled.GetHashCode();
                if (this.EventDataByteRateThreshold != null)
                    hashCode = hashCode * 59 + this.EventDataByteRateThreshold.GetHashCode();
                if (this.EventDataMsgRateThreshold != null)
                    hashCode = hashCode * 59 + this.EventDataMsgRateThreshold.GetHashCode();
                if (this.EventMaxMemoryThreshold != null)
                    hashCode = hashCode * 59 + this.EventMaxMemoryThreshold.GetHashCode();
                if (this.EventMaxTopicsThreshold != null)
                    hashCode = hashCode * 59 + this.EventMaxTopicsThreshold.GetHashCode();
                if (this.EventRequestQueueDepthThreshold != null)
                    hashCode = hashCode * 59 + this.EventRequestQueueDepthThreshold.GetHashCode();
                if (this.EventRequestRateThreshold != null)
                    hashCode = hashCode * 59 + this.EventRequestRateThreshold.GetHashCode();
                if (this.EventResponseRateThreshold != null)
                    hashCode = hashCode * 59 + this.EventResponseRateThreshold.GetHashCode();
                if (this.GlobalCachingEnabled != null)
                    hashCode = hashCode * 59 + this.GlobalCachingEnabled.GetHashCode();
                if (this.GlobalCachingHeartbeat != null)
                    hashCode = hashCode * 59 + this.GlobalCachingHeartbeat.GetHashCode();
                if (this.GlobalCachingTopicLifetime != null)
                    hashCode = hashCode * 59 + this.GlobalCachingTopicLifetime.GetHashCode();
                if (this.MaxMemory != null)
                    hashCode = hashCode * 59 + this.MaxMemory.GetHashCode();
                if (this.MaxMsgsPerTopic != null)
                    hashCode = hashCode * 59 + this.MaxMsgsPerTopic.GetHashCode();
                if (this.MaxRequestQueueDepth != null)
                    hashCode = hashCode * 59 + this.MaxRequestQueueDepth.GetHashCode();
                if (this.MaxTopicCount != null)
                    hashCode = hashCode * 59 + this.MaxTopicCount.GetHashCode();
                if (this.MsgLifetime != null)
                    hashCode = hashCode * 59 + this.MsgLifetime.GetHashCode();
                if (this.MsgVpnName != null)
                    hashCode = hashCode * 59 + this.MsgVpnName.GetHashCode();
                if (this.NewTopicAdvertisementEnabled != null)
                    hashCode = hashCode * 59 + this.NewTopicAdvertisementEnabled.GetHashCode();
                return hashCode;
            }
        }
    }
}
