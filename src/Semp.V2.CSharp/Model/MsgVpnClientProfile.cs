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
    /// MsgVpnClientProfile
    /// </summary>
    [DataContract]
        public partial class MsgVpnClientProfile :  IEquatable<MsgVpnClientProfile>
    {
        /// <summary>
        /// The types of Queues and Topic Endpoints that clients using the client-profile can create. Changing this value does not affect existing client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;all\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;all\&quot; - Client can create any type of endpoint. \&quot;durable\&quot; - Client can create only durable endpoints. \&quot;non-durable\&quot; - Client can create only non-durable endpoints. &lt;/pre&gt;  Available since 2.14.
        /// </summary>
        /// <value>The types of Queues and Topic Endpoints that clients using the client-profile can create. Changing this value does not affect existing client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;all\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;all\&quot; - Client can create any type of endpoint. \&quot;durable\&quot; - Client can create only durable endpoints. \&quot;non-durable\&quot; - Client can create only non-durable endpoints. &lt;/pre&gt;  Available since 2.14.</value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum AllowGuaranteedEndpointCreateDurabilityEnum
        {
            /// <summary>
            /// Enum All for value: all
            /// </summary>
            [EnumMember(Value = "all")]
            All = 1,
            /// <summary>
            /// Enum Durable for value: durable
            /// </summary>
            [EnumMember(Value = "durable")]
            Durable = 2,
            /// <summary>
            /// Enum NonDurable for value: non-durable
            /// </summary>
            [EnumMember(Value = "non-durable")]
            NonDurable = 3        }
        /// <summary>
        /// The types of Queues and Topic Endpoints that clients using the client-profile can create. Changing this value does not affect existing client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;all\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;all\&quot; - Client can create any type of endpoint. \&quot;durable\&quot; - Client can create only durable endpoints. \&quot;non-durable\&quot; - Client can create only non-durable endpoints. &lt;/pre&gt;  Available since 2.14.
        /// </summary>
        /// <value>The types of Queues and Topic Endpoints that clients using the client-profile can create. Changing this value does not affect existing client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;all\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;all\&quot; - Client can create any type of endpoint. \&quot;durable\&quot; - Client can create only durable endpoints. \&quot;non-durable\&quot; - Client can create only non-durable endpoints. &lt;/pre&gt;  Available since 2.14.</value>
        [DataMember(Name="allowGuaranteedEndpointCreateDurability", EmitDefaultValue=false)]
        public AllowGuaranteedEndpointCreateDurabilityEnum? AllowGuaranteedEndpointCreateDurability { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="MsgVpnClientProfile" /> class.
        /// </summary>
        /// <param name="allowBridgeConnectionsEnabled">Enable or disable allowing Bridge clients using the Client Profile to connect. Changing this setting does not affect existing Bridge client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="allowCutThroughForwardingEnabled">Enable or disable allowing clients using the Client Profile to bind to endpoints with the cut-through forwarding delivery mode. Changing this value does not affect existing client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.22. This attribute has been deprecated. Please visit the Solace Product Lifecycle Policy web page for details on deprecated features..</param>
        /// <param name="allowGuaranteedEndpointCreateDurability">The types of Queues and Topic Endpoints that clients using the client-profile can create. Changing this value does not affect existing client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;all\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;all\&quot; - Client can create any type of endpoint. \&quot;durable\&quot; - Client can create only durable endpoints. \&quot;non-durable\&quot; - Client can create only non-durable endpoints. &lt;/pre&gt;  Available since 2.14..</param>
        /// <param name="allowGuaranteedEndpointCreateEnabled">Enable or disable allowing clients using the Client Profile to create topic endponts or queues. Changing this value does not affect existing client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="allowGuaranteedMsgReceiveEnabled">Enable or disable allowing clients using the Client Profile to receive guaranteed messages. Changing this setting does not affect existing client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="allowGuaranteedMsgSendEnabled">Enable or disable allowing clients using the Client Profile to send guaranteed messages. Changing this setting does not affect existing client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="allowSharedSubscriptionsEnabled">Enable or disable allowing shared subscriptions. Changing this setting does not affect existing subscriptions. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.11..</param>
        /// <param name="allowTransactedSessionsEnabled">Enable or disable allowing clients using the Client Profile to establish transacted sessions. Changing this setting does not affect existing client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="apiQueueManagementCopyFromOnCreateName">The name of a queue to copy settings from when a new queue is created by a client using the Client Profile. The referenced queue must exist in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.14. This attribute has been replaced with &#x60;apiQueueManagementCopyFromOnCreateTemplateName&#x60;..</param>
        /// <param name="apiQueueManagementCopyFromOnCreateTemplateName">The name of a queue template to copy settings from when a new queue is created by a client using the Client Profile. If the referenced queue template does not exist, queue creation will fail when it tries to resolve this template. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.14..</param>
        /// <param name="apiTopicEndpointManagementCopyFromOnCreateName">The name of a topic endpoint to copy settings from when a new topic endpoint is created by a client using the Client Profile. The referenced topic endpoint must exist in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.14. This attribute has been replaced with &#x60;apiTopicEndpointManagementCopyFromOnCreateTemplateName&#x60;..</param>
        /// <param name="apiTopicEndpointManagementCopyFromOnCreateTemplateName">The name of a topic endpoint template to copy settings from when a new topic endpoint is created by a client using the Client Profile. If the referenced topic endpoint template does not exist, topic endpoint creation will fail when it tries to resolve this template. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.14..</param>
        /// <param name="clientProfileName">The name of the Client Profile..</param>
        /// <param name="compressionEnabled">Enable or disable allowing clients using the Client Profile to use compression. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;. Available since 2.10..</param>
        /// <param name="elidingDelay">The amount of time to delay the delivery of messages to clients using the Client Profile after the initial message has been delivered (the eliding delay interval), in milliseconds. A value of 0 means there is no delay in delivering messages to clients. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;..</param>
        /// <param name="elidingEnabled">Enable or disable message eliding for clients using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="elidingMaxTopicCount">The maximum number of topics tracked for message eliding per client connection using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;256&#x60;..</param>
        /// <param name="eventClientProvisionedEndpointSpoolUsageThreshold">eventClientProvisionedEndpointSpoolUsageThreshold.</param>
        /// <param name="eventConnectionCountPerClientUsernameThreshold">eventConnectionCountPerClientUsernameThreshold.</param>
        /// <param name="eventEgressFlowCountThreshold">eventEgressFlowCountThreshold.</param>
        /// <param name="eventEndpointCountPerClientUsernameThreshold">eventEndpointCountPerClientUsernameThreshold.</param>
        /// <param name="eventIngressFlowCountThreshold">eventIngressFlowCountThreshold.</param>
        /// <param name="eventServiceSmfConnectionCountPerClientUsernameThreshold">eventServiceSmfConnectionCountPerClientUsernameThreshold.</param>
        /// <param name="eventServiceWebConnectionCountPerClientUsernameThreshold">eventServiceWebConnectionCountPerClientUsernameThreshold.</param>
        /// <param name="eventSubscriptionCountThreshold">eventSubscriptionCountThreshold.</param>
        /// <param name="eventTransactedSessionCountThreshold">eventTransactedSessionCountThreshold.</param>
        /// <param name="eventTransactionCountThreshold">eventTransactionCountThreshold.</param>
        /// <param name="maxConnectionCountPerClientUsername">The maximum number of client connections per Client Username using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform..</param>
        /// <param name="maxEgressFlowCount">The maximum number of transmit flows that can be created by one client using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;..</param>
        /// <param name="maxEndpointCountPerClientUsername">The maximum number of queues and topic endpoints that can be created by clients with the same Client Username using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;..</param>
        /// <param name="maxIngressFlowCount">The maximum number of receive flows that can be created by one client using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;..</param>
        /// <param name="maxMsgsPerTransaction">The maximum number of publisher and consumer messages combined that is allowed within a transaction for each client associated with this client-profile. Exceeding this limit will result in a transaction prepare or commit failure. Changing this value during operation will not affect existing sessions. It is only validated at transaction creation time. Large transactions consume more resources and are more likely to require retrieving messages from the ADB or from disk to process the transaction prepare or commit requests. The transaction processing rate may diminish if a large number of messages must be retrieved from the ADB or from disk. Care should be taken to not use excessively large transactions needlessly to avoid exceeding resource limits and to avoid reducing the overall broker performance. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;256&#x60;. Available since 2.20..</param>
        /// <param name="maxSubscriptionCount">The maximum number of subscriptions per client using the Client Profile. This limit is not enforced when a client adds a subscription to an endpoint, except for MQTT QoS 1 subscriptions. In addition, this limit is not enforced when a subscription is added using a management interface, such as CLI or SEMP. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default varies by platform..</param>
        /// <param name="maxTransactedSessionCount">The maximum number of transacted sessions that can be created by one client using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;10&#x60;..</param>
        /// <param name="maxTransactionCount">The maximum number of transactions that can be created by one client using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default varies by platform..</param>
        /// <param name="msgVpnName">The name of the Message VPN..</param>
        /// <param name="queueControl1MaxDepth">The maximum depth of the \&quot;Control 1\&quot; (C-1) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;20000&#x60;..</param>
        /// <param name="queueControl1MinMsgBurst">The number of messages that are always allowed entry into the \&quot;Control 1\&quot; (C-1) priority queue, regardless of the &#x60;queueControl1MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;4&#x60;..</param>
        /// <param name="queueDirect1MaxDepth">The maximum depth of the \&quot;Direct 1\&quot; (D-1) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;20000&#x60;..</param>
        /// <param name="queueDirect1MinMsgBurst">The number of messages that are always allowed entry into the \&quot;Direct 1\&quot; (D-1) priority queue, regardless of the &#x60;queueDirect1MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;4&#x60;..</param>
        /// <param name="queueDirect2MaxDepth">The maximum depth of the \&quot;Direct 2\&quot; (D-2) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;20000&#x60;..</param>
        /// <param name="queueDirect2MinMsgBurst">The number of messages that are always allowed entry into the \&quot;Direct 2\&quot; (D-2) priority queue, regardless of the &#x60;queueDirect2MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;4&#x60;..</param>
        /// <param name="queueDirect3MaxDepth">The maximum depth of the \&quot;Direct 3\&quot; (D-3) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;20000&#x60;..</param>
        /// <param name="queueDirect3MinMsgBurst">The number of messages that are always allowed entry into the \&quot;Direct 3\&quot; (D-3) priority queue, regardless of the &#x60;queueDirect3MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;4&#x60;..</param>
        /// <param name="queueGuaranteed1MaxDepth">The maximum depth of the \&quot;Guaranteed 1\&quot; (G-1) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;20000&#x60;..</param>
        /// <param name="queueGuaranteed1MinMsgBurst">The number of messages that are always allowed entry into the \&quot;Guaranteed 1\&quot; (G-3) priority queue, regardless of the &#x60;queueGuaranteed1MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;255&#x60;..</param>
        /// <param name="rejectMsgToSenderOnNoSubscriptionMatchEnabled">Enable or disable the sending of a negative acknowledgement (NACK) to a client using the Client Profile when discarding a guaranteed message due to no matching subscription found. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.2..</param>
        /// <param name="replicationAllowClientConnectWhenStandbyEnabled">Enable or disable allowing clients using the Client Profile to connect to the Message VPN when its replication state is standby. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="serviceMinKeepaliveTimeout">The minimum client keepalive timeout which will be enforced for client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;30&#x60;. Available since 2.19..</param>
        /// <param name="serviceSmfMaxConnectionCountPerClientUsername">The maximum number of SMF client connections per Client Username using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform..</param>
        /// <param name="serviceSmfMinKeepaliveEnabled">Enable or disable the enforcement of a minimum keepalive timeout for SMF clients. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.19..</param>
        /// <param name="serviceWebInactiveTimeout">The timeout for inactive Web Transport client sessions using the Client Profile, in seconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;30&#x60;..</param>
        /// <param name="serviceWebMaxConnectionCountPerClientUsername">The maximum number of Web Transport client connections per Client Username using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform..</param>
        /// <param name="serviceWebMaxPayload">The maximum Web Transport payload size before fragmentation occurs for clients using the Client Profile, in bytes. The size of the header is not included. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000000&#x60;..</param>
        /// <param name="tcpCongestionWindowSize">The TCP initial congestion window size for clients using the Client Profile, in multiples of the TCP Maximum Segment Size (MSS). Changing the value from its default of 2 results in non-compliance with RFC 2581. Contact support before changing this value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;2&#x60;..</param>
        /// <param name="tcpKeepaliveCount">The number of TCP keepalive retransmissions to a client using the Client Profile before declaring that it is not available. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;5&#x60;..</param>
        /// <param name="tcpKeepaliveIdleTime">The amount of time a client connection using the Client Profile must remain idle before TCP begins sending keepalive probes, in seconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;..</param>
        /// <param name="tcpKeepaliveInterval">The amount of time between TCP keepalive retransmissions to a client using the Client Profile when no acknowledgement is received, in seconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1&#x60;..</param>
        /// <param name="tcpMaxSegmentSize">The TCP maximum segment size for clients using the Client Profile, in bytes. Changes are applied to all existing connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1460&#x60;..</param>
        /// <param name="tcpMaxWindowSize">The TCP maximum window size for clients using the Client Profile, in kilobytes. Changes are applied to all existing connections. This setting is ignored on the software broker. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;256&#x60;..</param>
        /// <param name="tlsAllowDowngradeToPlainTextEnabled">Enable or disable allowing a client using the Client Profile to downgrade an encrypted connection to plain text. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;. Available since 2.8..</param>
        public MsgVpnClientProfile(bool? allowBridgeConnectionsEnabled = default(bool?), bool? allowCutThroughForwardingEnabled = default(bool?), AllowGuaranteedEndpointCreateDurabilityEnum? allowGuaranteedEndpointCreateDurability = default(AllowGuaranteedEndpointCreateDurabilityEnum?), bool? allowGuaranteedEndpointCreateEnabled = default(bool?), bool? allowGuaranteedMsgReceiveEnabled = default(bool?), bool? allowGuaranteedMsgSendEnabled = default(bool?), bool? allowSharedSubscriptionsEnabled = default(bool?), bool? allowTransactedSessionsEnabled = default(bool?), string apiQueueManagementCopyFromOnCreateName = default(string), string apiQueueManagementCopyFromOnCreateTemplateName = default(string), string apiTopicEndpointManagementCopyFromOnCreateName = default(string), string apiTopicEndpointManagementCopyFromOnCreateTemplateName = default(string), string clientProfileName = default(string), bool? compressionEnabled = default(bool?), long? elidingDelay = default(long?), bool? elidingEnabled = default(bool?), long? elidingMaxTopicCount = default(long?), EventThresholdByPercent eventClientProvisionedEndpointSpoolUsageThreshold = default(EventThresholdByPercent), EventThreshold eventConnectionCountPerClientUsernameThreshold = default(EventThreshold), EventThreshold eventEgressFlowCountThreshold = default(EventThreshold), EventThreshold eventEndpointCountPerClientUsernameThreshold = default(EventThreshold), EventThreshold eventIngressFlowCountThreshold = default(EventThreshold), EventThreshold eventServiceSmfConnectionCountPerClientUsernameThreshold = default(EventThreshold), EventThreshold eventServiceWebConnectionCountPerClientUsernameThreshold = default(EventThreshold), EventThreshold eventSubscriptionCountThreshold = default(EventThreshold), EventThreshold eventTransactedSessionCountThreshold = default(EventThreshold), EventThreshold eventTransactionCountThreshold = default(EventThreshold), long? maxConnectionCountPerClientUsername = default(long?), long? maxEgressFlowCount = default(long?), long? maxEndpointCountPerClientUsername = default(long?), long? maxIngressFlowCount = default(long?), int? maxMsgsPerTransaction = default(int?), long? maxSubscriptionCount = default(long?), long? maxTransactedSessionCount = default(long?), long? maxTransactionCount = default(long?), string msgVpnName = default(string), int? queueControl1MaxDepth = default(int?), int? queueControl1MinMsgBurst = default(int?), int? queueDirect1MaxDepth = default(int?), int? queueDirect1MinMsgBurst = default(int?), int? queueDirect2MaxDepth = default(int?), int? queueDirect2MinMsgBurst = default(int?), int? queueDirect3MaxDepth = default(int?), int? queueDirect3MinMsgBurst = default(int?), int? queueGuaranteed1MaxDepth = default(int?), int? queueGuaranteed1MinMsgBurst = default(int?), bool? rejectMsgToSenderOnNoSubscriptionMatchEnabled = default(bool?), bool? replicationAllowClientConnectWhenStandbyEnabled = default(bool?), int? serviceMinKeepaliveTimeout = default(int?), long? serviceSmfMaxConnectionCountPerClientUsername = default(long?), bool? serviceSmfMinKeepaliveEnabled = default(bool?), long? serviceWebInactiveTimeout = default(long?), long? serviceWebMaxConnectionCountPerClientUsername = default(long?), long? serviceWebMaxPayload = default(long?), long? tcpCongestionWindowSize = default(long?), long? tcpKeepaliveCount = default(long?), long? tcpKeepaliveIdleTime = default(long?), long? tcpKeepaliveInterval = default(long?), long? tcpMaxSegmentSize = default(long?), long? tcpMaxWindowSize = default(long?), bool? tlsAllowDowngradeToPlainTextEnabled = default(bool?))
        {
            this.AllowBridgeConnectionsEnabled = allowBridgeConnectionsEnabled;
            this.AllowCutThroughForwardingEnabled = allowCutThroughForwardingEnabled;
            this.AllowGuaranteedEndpointCreateDurability = allowGuaranteedEndpointCreateDurability;
            this.AllowGuaranteedEndpointCreateEnabled = allowGuaranteedEndpointCreateEnabled;
            this.AllowGuaranteedMsgReceiveEnabled = allowGuaranteedMsgReceiveEnabled;
            this.AllowGuaranteedMsgSendEnabled = allowGuaranteedMsgSendEnabled;
            this.AllowSharedSubscriptionsEnabled = allowSharedSubscriptionsEnabled;
            this.AllowTransactedSessionsEnabled = allowTransactedSessionsEnabled;
            this.ApiQueueManagementCopyFromOnCreateName = apiQueueManagementCopyFromOnCreateName;
            this.ApiQueueManagementCopyFromOnCreateTemplateName = apiQueueManagementCopyFromOnCreateTemplateName;
            this.ApiTopicEndpointManagementCopyFromOnCreateName = apiTopicEndpointManagementCopyFromOnCreateName;
            this.ApiTopicEndpointManagementCopyFromOnCreateTemplateName = apiTopicEndpointManagementCopyFromOnCreateTemplateName;
            this.ClientProfileName = clientProfileName;
            this.CompressionEnabled = compressionEnabled;
            this.ElidingDelay = elidingDelay;
            this.ElidingEnabled = elidingEnabled;
            this.ElidingMaxTopicCount = elidingMaxTopicCount;
            this.EventClientProvisionedEndpointSpoolUsageThreshold = eventClientProvisionedEndpointSpoolUsageThreshold;
            this.EventConnectionCountPerClientUsernameThreshold = eventConnectionCountPerClientUsernameThreshold;
            this.EventEgressFlowCountThreshold = eventEgressFlowCountThreshold;
            this.EventEndpointCountPerClientUsernameThreshold = eventEndpointCountPerClientUsernameThreshold;
            this.EventIngressFlowCountThreshold = eventIngressFlowCountThreshold;
            this.EventServiceSmfConnectionCountPerClientUsernameThreshold = eventServiceSmfConnectionCountPerClientUsernameThreshold;
            this.EventServiceWebConnectionCountPerClientUsernameThreshold = eventServiceWebConnectionCountPerClientUsernameThreshold;
            this.EventSubscriptionCountThreshold = eventSubscriptionCountThreshold;
            this.EventTransactedSessionCountThreshold = eventTransactedSessionCountThreshold;
            this.EventTransactionCountThreshold = eventTransactionCountThreshold;
            this.MaxConnectionCountPerClientUsername = maxConnectionCountPerClientUsername;
            this.MaxEgressFlowCount = maxEgressFlowCount;
            this.MaxEndpointCountPerClientUsername = maxEndpointCountPerClientUsername;
            this.MaxIngressFlowCount = maxIngressFlowCount;
            this.MaxMsgsPerTransaction = maxMsgsPerTransaction;
            this.MaxSubscriptionCount = maxSubscriptionCount;
            this.MaxTransactedSessionCount = maxTransactedSessionCount;
            this.MaxTransactionCount = maxTransactionCount;
            this.MsgVpnName = msgVpnName;
            this.QueueControl1MaxDepth = queueControl1MaxDepth;
            this.QueueControl1MinMsgBurst = queueControl1MinMsgBurst;
            this.QueueDirect1MaxDepth = queueDirect1MaxDepth;
            this.QueueDirect1MinMsgBurst = queueDirect1MinMsgBurst;
            this.QueueDirect2MaxDepth = queueDirect2MaxDepth;
            this.QueueDirect2MinMsgBurst = queueDirect2MinMsgBurst;
            this.QueueDirect3MaxDepth = queueDirect3MaxDepth;
            this.QueueDirect3MinMsgBurst = queueDirect3MinMsgBurst;
            this.QueueGuaranteed1MaxDepth = queueGuaranteed1MaxDepth;
            this.QueueGuaranteed1MinMsgBurst = queueGuaranteed1MinMsgBurst;
            this.RejectMsgToSenderOnNoSubscriptionMatchEnabled = rejectMsgToSenderOnNoSubscriptionMatchEnabled;
            this.ReplicationAllowClientConnectWhenStandbyEnabled = replicationAllowClientConnectWhenStandbyEnabled;
            this.ServiceMinKeepaliveTimeout = serviceMinKeepaliveTimeout;
            this.ServiceSmfMaxConnectionCountPerClientUsername = serviceSmfMaxConnectionCountPerClientUsername;
            this.ServiceSmfMinKeepaliveEnabled = serviceSmfMinKeepaliveEnabled;
            this.ServiceWebInactiveTimeout = serviceWebInactiveTimeout;
            this.ServiceWebMaxConnectionCountPerClientUsername = serviceWebMaxConnectionCountPerClientUsername;
            this.ServiceWebMaxPayload = serviceWebMaxPayload;
            this.TcpCongestionWindowSize = tcpCongestionWindowSize;
            this.TcpKeepaliveCount = tcpKeepaliveCount;
            this.TcpKeepaliveIdleTime = tcpKeepaliveIdleTime;
            this.TcpKeepaliveInterval = tcpKeepaliveInterval;
            this.TcpMaxSegmentSize = tcpMaxSegmentSize;
            this.TcpMaxWindowSize = tcpMaxWindowSize;
            this.TlsAllowDowngradeToPlainTextEnabled = tlsAllowDowngradeToPlainTextEnabled;
        }
        
        /// <summary>
        /// Enable or disable allowing Bridge clients using the Client Profile to connect. Changing this setting does not affect existing Bridge client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable allowing Bridge clients using the Client Profile to connect. Changing this setting does not affect existing Bridge client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="allowBridgeConnectionsEnabled", EmitDefaultValue=false)]
        public bool? AllowBridgeConnectionsEnabled { get; set; }

        /// <summary>
        /// Enable or disable allowing clients using the Client Profile to bind to endpoints with the cut-through forwarding delivery mode. Changing this value does not affect existing client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.22. This attribute has been deprecated. Please visit the Solace Product Lifecycle Policy web page for details on deprecated features.
        /// </summary>
        /// <value>Enable or disable allowing clients using the Client Profile to bind to endpoints with the cut-through forwarding delivery mode. Changing this value does not affect existing client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.22. This attribute has been deprecated. Please visit the Solace Product Lifecycle Policy web page for details on deprecated features.</value>
        [DataMember(Name="allowCutThroughForwardingEnabled", EmitDefaultValue=false)]
        public bool? AllowCutThroughForwardingEnabled { get; set; }


        /// <summary>
        /// Enable or disable allowing clients using the Client Profile to create topic endponts or queues. Changing this value does not affect existing client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable allowing clients using the Client Profile to create topic endponts or queues. Changing this value does not affect existing client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="allowGuaranteedEndpointCreateEnabled", EmitDefaultValue=false)]
        public bool? AllowGuaranteedEndpointCreateEnabled { get; set; }

        /// <summary>
        /// Enable or disable allowing clients using the Client Profile to receive guaranteed messages. Changing this setting does not affect existing client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable allowing clients using the Client Profile to receive guaranteed messages. Changing this setting does not affect existing client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="allowGuaranteedMsgReceiveEnabled", EmitDefaultValue=false)]
        public bool? AllowGuaranteedMsgReceiveEnabled { get; set; }

        /// <summary>
        /// Enable or disable allowing clients using the Client Profile to send guaranteed messages. Changing this setting does not affect existing client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable allowing clients using the Client Profile to send guaranteed messages. Changing this setting does not affect existing client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="allowGuaranteedMsgSendEnabled", EmitDefaultValue=false)]
        public bool? AllowGuaranteedMsgSendEnabled { get; set; }

        /// <summary>
        /// Enable or disable allowing shared subscriptions. Changing this setting does not affect existing subscriptions. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.11.
        /// </summary>
        /// <value>Enable or disable allowing shared subscriptions. Changing this setting does not affect existing subscriptions. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.11.</value>
        [DataMember(Name="allowSharedSubscriptionsEnabled", EmitDefaultValue=false)]
        public bool? AllowSharedSubscriptionsEnabled { get; set; }

        /// <summary>
        /// Enable or disable allowing clients using the Client Profile to establish transacted sessions. Changing this setting does not affect existing client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable allowing clients using the Client Profile to establish transacted sessions. Changing this setting does not affect existing client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="allowTransactedSessionsEnabled", EmitDefaultValue=false)]
        public bool? AllowTransactedSessionsEnabled { get; set; }

        /// <summary>
        /// The name of a queue to copy settings from when a new queue is created by a client using the Client Profile. The referenced queue must exist in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.14. This attribute has been replaced with &#x60;apiQueueManagementCopyFromOnCreateTemplateName&#x60;.
        /// </summary>
        /// <value>The name of a queue to copy settings from when a new queue is created by a client using the Client Profile. The referenced queue must exist in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.14. This attribute has been replaced with &#x60;apiQueueManagementCopyFromOnCreateTemplateName&#x60;.</value>
        [DataMember(Name="apiQueueManagementCopyFromOnCreateName", EmitDefaultValue=false)]
        public string ApiQueueManagementCopyFromOnCreateName { get; set; }

        /// <summary>
        /// The name of a queue template to copy settings from when a new queue is created by a client using the Client Profile. If the referenced queue template does not exist, queue creation will fail when it tries to resolve this template. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.14.
        /// </summary>
        /// <value>The name of a queue template to copy settings from when a new queue is created by a client using the Client Profile. If the referenced queue template does not exist, queue creation will fail when it tries to resolve this template. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.14.</value>
        [DataMember(Name="apiQueueManagementCopyFromOnCreateTemplateName", EmitDefaultValue=false)]
        public string ApiQueueManagementCopyFromOnCreateTemplateName { get; set; }

        /// <summary>
        /// The name of a topic endpoint to copy settings from when a new topic endpoint is created by a client using the Client Profile. The referenced topic endpoint must exist in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.14. This attribute has been replaced with &#x60;apiTopicEndpointManagementCopyFromOnCreateTemplateName&#x60;.
        /// </summary>
        /// <value>The name of a topic endpoint to copy settings from when a new topic endpoint is created by a client using the Client Profile. The referenced topic endpoint must exist in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.14. This attribute has been replaced with &#x60;apiTopicEndpointManagementCopyFromOnCreateTemplateName&#x60;.</value>
        [DataMember(Name="apiTopicEndpointManagementCopyFromOnCreateName", EmitDefaultValue=false)]
        public string ApiTopicEndpointManagementCopyFromOnCreateName { get; set; }

        /// <summary>
        /// The name of a topic endpoint template to copy settings from when a new topic endpoint is created by a client using the Client Profile. If the referenced topic endpoint template does not exist, topic endpoint creation will fail when it tries to resolve this template. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.14.
        /// </summary>
        /// <value>The name of a topic endpoint template to copy settings from when a new topic endpoint is created by a client using the Client Profile. If the referenced topic endpoint template does not exist, topic endpoint creation will fail when it tries to resolve this template. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.14.</value>
        [DataMember(Name="apiTopicEndpointManagementCopyFromOnCreateTemplateName", EmitDefaultValue=false)]
        public string ApiTopicEndpointManagementCopyFromOnCreateTemplateName { get; set; }

        /// <summary>
        /// The name of the Client Profile.
        /// </summary>
        /// <value>The name of the Client Profile.</value>
        [DataMember(Name="clientProfileName", EmitDefaultValue=false)]
        public string ClientProfileName { get; set; }

        /// <summary>
        /// Enable or disable allowing clients using the Client Profile to use compression. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;. Available since 2.10.
        /// </summary>
        /// <value>Enable or disable allowing clients using the Client Profile to use compression. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;. Available since 2.10.</value>
        [DataMember(Name="compressionEnabled", EmitDefaultValue=false)]
        public bool? CompressionEnabled { get; set; }

        /// <summary>
        /// The amount of time to delay the delivery of messages to clients using the Client Profile after the initial message has been delivered (the eliding delay interval), in milliseconds. A value of 0 means there is no delay in delivering messages to clients. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;.
        /// </summary>
        /// <value>The amount of time to delay the delivery of messages to clients using the Client Profile after the initial message has been delivered (the eliding delay interval), in milliseconds. A value of 0 means there is no delay in delivering messages to clients. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;.</value>
        [DataMember(Name="elidingDelay", EmitDefaultValue=false)]
        public long? ElidingDelay { get; set; }

        /// <summary>
        /// Enable or disable message eliding for clients using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable message eliding for clients using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="elidingEnabled", EmitDefaultValue=false)]
        public bool? ElidingEnabled { get; set; }

        /// <summary>
        /// The maximum number of topics tracked for message eliding per client connection using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;256&#x60;.
        /// </summary>
        /// <value>The maximum number of topics tracked for message eliding per client connection using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;256&#x60;.</value>
        [DataMember(Name="elidingMaxTopicCount", EmitDefaultValue=false)]
        public long? ElidingMaxTopicCount { get; set; }

        /// <summary>
        /// Gets or Sets EventClientProvisionedEndpointSpoolUsageThreshold
        /// </summary>
        [DataMember(Name="eventClientProvisionedEndpointSpoolUsageThreshold", EmitDefaultValue=false)]
        public EventThresholdByPercent EventClientProvisionedEndpointSpoolUsageThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventConnectionCountPerClientUsernameThreshold
        /// </summary>
        [DataMember(Name="eventConnectionCountPerClientUsernameThreshold", EmitDefaultValue=false)]
        public EventThreshold EventConnectionCountPerClientUsernameThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventEgressFlowCountThreshold
        /// </summary>
        [DataMember(Name="eventEgressFlowCountThreshold", EmitDefaultValue=false)]
        public EventThreshold EventEgressFlowCountThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventEndpointCountPerClientUsernameThreshold
        /// </summary>
        [DataMember(Name="eventEndpointCountPerClientUsernameThreshold", EmitDefaultValue=false)]
        public EventThreshold EventEndpointCountPerClientUsernameThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventIngressFlowCountThreshold
        /// </summary>
        [DataMember(Name="eventIngressFlowCountThreshold", EmitDefaultValue=false)]
        public EventThreshold EventIngressFlowCountThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventServiceSmfConnectionCountPerClientUsernameThreshold
        /// </summary>
        [DataMember(Name="eventServiceSmfConnectionCountPerClientUsernameThreshold", EmitDefaultValue=false)]
        public EventThreshold EventServiceSmfConnectionCountPerClientUsernameThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventServiceWebConnectionCountPerClientUsernameThreshold
        /// </summary>
        [DataMember(Name="eventServiceWebConnectionCountPerClientUsernameThreshold", EmitDefaultValue=false)]
        public EventThreshold EventServiceWebConnectionCountPerClientUsernameThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventSubscriptionCountThreshold
        /// </summary>
        [DataMember(Name="eventSubscriptionCountThreshold", EmitDefaultValue=false)]
        public EventThreshold EventSubscriptionCountThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventTransactedSessionCountThreshold
        /// </summary>
        [DataMember(Name="eventTransactedSessionCountThreshold", EmitDefaultValue=false)]
        public EventThreshold EventTransactedSessionCountThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventTransactionCountThreshold
        /// </summary>
        [DataMember(Name="eventTransactionCountThreshold", EmitDefaultValue=false)]
        public EventThreshold EventTransactionCountThreshold { get; set; }

        /// <summary>
        /// The maximum number of client connections per Client Username using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform.
        /// </summary>
        /// <value>The maximum number of client connections per Client Username using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform.</value>
        [DataMember(Name="maxConnectionCountPerClientUsername", EmitDefaultValue=false)]
        public long? MaxConnectionCountPerClientUsername { get; set; }

        /// <summary>
        /// The maximum number of transmit flows that can be created by one client using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;.
        /// </summary>
        /// <value>The maximum number of transmit flows that can be created by one client using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;.</value>
        [DataMember(Name="maxEgressFlowCount", EmitDefaultValue=false)]
        public long? MaxEgressFlowCount { get; set; }

        /// <summary>
        /// The maximum number of queues and topic endpoints that can be created by clients with the same Client Username using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;.
        /// </summary>
        /// <value>The maximum number of queues and topic endpoints that can be created by clients with the same Client Username using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;.</value>
        [DataMember(Name="maxEndpointCountPerClientUsername", EmitDefaultValue=false)]
        public long? MaxEndpointCountPerClientUsername { get; set; }

        /// <summary>
        /// The maximum number of receive flows that can be created by one client using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;.
        /// </summary>
        /// <value>The maximum number of receive flows that can be created by one client using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;.</value>
        [DataMember(Name="maxIngressFlowCount", EmitDefaultValue=false)]
        public long? MaxIngressFlowCount { get; set; }

        /// <summary>
        /// The maximum number of publisher and consumer messages combined that is allowed within a transaction for each client associated with this client-profile. Exceeding this limit will result in a transaction prepare or commit failure. Changing this value during operation will not affect existing sessions. It is only validated at transaction creation time. Large transactions consume more resources and are more likely to require retrieving messages from the ADB or from disk to process the transaction prepare or commit requests. The transaction processing rate may diminish if a large number of messages must be retrieved from the ADB or from disk. Care should be taken to not use excessively large transactions needlessly to avoid exceeding resource limits and to avoid reducing the overall broker performance. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;256&#x60;. Available since 2.20.
        /// </summary>
        /// <value>The maximum number of publisher and consumer messages combined that is allowed within a transaction for each client associated with this client-profile. Exceeding this limit will result in a transaction prepare or commit failure. Changing this value during operation will not affect existing sessions. It is only validated at transaction creation time. Large transactions consume more resources and are more likely to require retrieving messages from the ADB or from disk to process the transaction prepare or commit requests. The transaction processing rate may diminish if a large number of messages must be retrieved from the ADB or from disk. Care should be taken to not use excessively large transactions needlessly to avoid exceeding resource limits and to avoid reducing the overall broker performance. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;256&#x60;. Available since 2.20.</value>
        [DataMember(Name="maxMsgsPerTransaction", EmitDefaultValue=false)]
        public int? MaxMsgsPerTransaction { get; set; }

        /// <summary>
        /// The maximum number of subscriptions per client using the Client Profile. This limit is not enforced when a client adds a subscription to an endpoint, except for MQTT QoS 1 subscriptions. In addition, this limit is not enforced when a subscription is added using a management interface, such as CLI or SEMP. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default varies by platform.
        /// </summary>
        /// <value>The maximum number of subscriptions per client using the Client Profile. This limit is not enforced when a client adds a subscription to an endpoint, except for MQTT QoS 1 subscriptions. In addition, this limit is not enforced when a subscription is added using a management interface, such as CLI or SEMP. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default varies by platform.</value>
        [DataMember(Name="maxSubscriptionCount", EmitDefaultValue=false)]
        public long? MaxSubscriptionCount { get; set; }

        /// <summary>
        /// The maximum number of transacted sessions that can be created by one client using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;10&#x60;.
        /// </summary>
        /// <value>The maximum number of transacted sessions that can be created by one client using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;10&#x60;.</value>
        [DataMember(Name="maxTransactedSessionCount", EmitDefaultValue=false)]
        public long? MaxTransactedSessionCount { get; set; }

        /// <summary>
        /// The maximum number of transactions that can be created by one client using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default varies by platform.
        /// </summary>
        /// <value>The maximum number of transactions that can be created by one client using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default varies by platform.</value>
        [DataMember(Name="maxTransactionCount", EmitDefaultValue=false)]
        public long? MaxTransactionCount { get; set; }

        /// <summary>
        /// The name of the Message VPN.
        /// </summary>
        /// <value>The name of the Message VPN.</value>
        [DataMember(Name="msgVpnName", EmitDefaultValue=false)]
        public string MsgVpnName { get; set; }

        /// <summary>
        /// The maximum depth of the \&quot;Control 1\&quot; (C-1) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;20000&#x60;.
        /// </summary>
        /// <value>The maximum depth of the \&quot;Control 1\&quot; (C-1) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;20000&#x60;.</value>
        [DataMember(Name="queueControl1MaxDepth", EmitDefaultValue=false)]
        public int? QueueControl1MaxDepth { get; set; }

        /// <summary>
        /// The number of messages that are always allowed entry into the \&quot;Control 1\&quot; (C-1) priority queue, regardless of the &#x60;queueControl1MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;4&#x60;.
        /// </summary>
        /// <value>The number of messages that are always allowed entry into the \&quot;Control 1\&quot; (C-1) priority queue, regardless of the &#x60;queueControl1MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;4&#x60;.</value>
        [DataMember(Name="queueControl1MinMsgBurst", EmitDefaultValue=false)]
        public int? QueueControl1MinMsgBurst { get; set; }

        /// <summary>
        /// The maximum depth of the \&quot;Direct 1\&quot; (D-1) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;20000&#x60;.
        /// </summary>
        /// <value>The maximum depth of the \&quot;Direct 1\&quot; (D-1) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;20000&#x60;.</value>
        [DataMember(Name="queueDirect1MaxDepth", EmitDefaultValue=false)]
        public int? QueueDirect1MaxDepth { get; set; }

        /// <summary>
        /// The number of messages that are always allowed entry into the \&quot;Direct 1\&quot; (D-1) priority queue, regardless of the &#x60;queueDirect1MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;4&#x60;.
        /// </summary>
        /// <value>The number of messages that are always allowed entry into the \&quot;Direct 1\&quot; (D-1) priority queue, regardless of the &#x60;queueDirect1MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;4&#x60;.</value>
        [DataMember(Name="queueDirect1MinMsgBurst", EmitDefaultValue=false)]
        public int? QueueDirect1MinMsgBurst { get; set; }

        /// <summary>
        /// The maximum depth of the \&quot;Direct 2\&quot; (D-2) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;20000&#x60;.
        /// </summary>
        /// <value>The maximum depth of the \&quot;Direct 2\&quot; (D-2) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;20000&#x60;.</value>
        [DataMember(Name="queueDirect2MaxDepth", EmitDefaultValue=false)]
        public int? QueueDirect2MaxDepth { get; set; }

        /// <summary>
        /// The number of messages that are always allowed entry into the \&quot;Direct 2\&quot; (D-2) priority queue, regardless of the &#x60;queueDirect2MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;4&#x60;.
        /// </summary>
        /// <value>The number of messages that are always allowed entry into the \&quot;Direct 2\&quot; (D-2) priority queue, regardless of the &#x60;queueDirect2MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;4&#x60;.</value>
        [DataMember(Name="queueDirect2MinMsgBurst", EmitDefaultValue=false)]
        public int? QueueDirect2MinMsgBurst { get; set; }

        /// <summary>
        /// The maximum depth of the \&quot;Direct 3\&quot; (D-3) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;20000&#x60;.
        /// </summary>
        /// <value>The maximum depth of the \&quot;Direct 3\&quot; (D-3) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;20000&#x60;.</value>
        [DataMember(Name="queueDirect3MaxDepth", EmitDefaultValue=false)]
        public int? QueueDirect3MaxDepth { get; set; }

        /// <summary>
        /// The number of messages that are always allowed entry into the \&quot;Direct 3\&quot; (D-3) priority queue, regardless of the &#x60;queueDirect3MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;4&#x60;.
        /// </summary>
        /// <value>The number of messages that are always allowed entry into the \&quot;Direct 3\&quot; (D-3) priority queue, regardless of the &#x60;queueDirect3MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;4&#x60;.</value>
        [DataMember(Name="queueDirect3MinMsgBurst", EmitDefaultValue=false)]
        public int? QueueDirect3MinMsgBurst { get; set; }

        /// <summary>
        /// The maximum depth of the \&quot;Guaranteed 1\&quot; (G-1) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;20000&#x60;.
        /// </summary>
        /// <value>The maximum depth of the \&quot;Guaranteed 1\&quot; (G-1) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;20000&#x60;.</value>
        [DataMember(Name="queueGuaranteed1MaxDepth", EmitDefaultValue=false)]
        public int? QueueGuaranteed1MaxDepth { get; set; }

        /// <summary>
        /// The number of messages that are always allowed entry into the \&quot;Guaranteed 1\&quot; (G-3) priority queue, regardless of the &#x60;queueGuaranteed1MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;255&#x60;.
        /// </summary>
        /// <value>The number of messages that are always allowed entry into the \&quot;Guaranteed 1\&quot; (G-3) priority queue, regardless of the &#x60;queueGuaranteed1MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;255&#x60;.</value>
        [DataMember(Name="queueGuaranteed1MinMsgBurst", EmitDefaultValue=false)]
        public int? QueueGuaranteed1MinMsgBurst { get; set; }

        /// <summary>
        /// Enable or disable the sending of a negative acknowledgement (NACK) to a client using the Client Profile when discarding a guaranteed message due to no matching subscription found. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.2.
        /// </summary>
        /// <value>Enable or disable the sending of a negative acknowledgement (NACK) to a client using the Client Profile when discarding a guaranteed message due to no matching subscription found. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.2.</value>
        [DataMember(Name="rejectMsgToSenderOnNoSubscriptionMatchEnabled", EmitDefaultValue=false)]
        public bool? RejectMsgToSenderOnNoSubscriptionMatchEnabled { get; set; }

        /// <summary>
        /// Enable or disable allowing clients using the Client Profile to connect to the Message VPN when its replication state is standby. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable allowing clients using the Client Profile to connect to the Message VPN when its replication state is standby. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="replicationAllowClientConnectWhenStandbyEnabled", EmitDefaultValue=false)]
        public bool? ReplicationAllowClientConnectWhenStandbyEnabled { get; set; }

        /// <summary>
        /// The minimum client keepalive timeout which will be enforced for client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;30&#x60;. Available since 2.19.
        /// </summary>
        /// <value>The minimum client keepalive timeout which will be enforced for client connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;30&#x60;. Available since 2.19.</value>
        [DataMember(Name="serviceMinKeepaliveTimeout", EmitDefaultValue=false)]
        public int? ServiceMinKeepaliveTimeout { get; set; }

        /// <summary>
        /// The maximum number of SMF client connections per Client Username using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform.
        /// </summary>
        /// <value>The maximum number of SMF client connections per Client Username using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform.</value>
        [DataMember(Name="serviceSmfMaxConnectionCountPerClientUsername", EmitDefaultValue=false)]
        public long? ServiceSmfMaxConnectionCountPerClientUsername { get; set; }

        /// <summary>
        /// Enable or disable the enforcement of a minimum keepalive timeout for SMF clients. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.19.
        /// </summary>
        /// <value>Enable or disable the enforcement of a minimum keepalive timeout for SMF clients. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.19.</value>
        [DataMember(Name="serviceSmfMinKeepaliveEnabled", EmitDefaultValue=false)]
        public bool? ServiceSmfMinKeepaliveEnabled { get; set; }

        /// <summary>
        /// The timeout for inactive Web Transport client sessions using the Client Profile, in seconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;30&#x60;.
        /// </summary>
        /// <value>The timeout for inactive Web Transport client sessions using the Client Profile, in seconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;30&#x60;.</value>
        [DataMember(Name="serviceWebInactiveTimeout", EmitDefaultValue=false)]
        public long? ServiceWebInactiveTimeout { get; set; }

        /// <summary>
        /// The maximum number of Web Transport client connections per Client Username using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform.
        /// </summary>
        /// <value>The maximum number of Web Transport client connections per Client Username using the Client Profile. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform.</value>
        [DataMember(Name="serviceWebMaxConnectionCountPerClientUsername", EmitDefaultValue=false)]
        public long? ServiceWebMaxConnectionCountPerClientUsername { get; set; }

        /// <summary>
        /// The maximum Web Transport payload size before fragmentation occurs for clients using the Client Profile, in bytes. The size of the header is not included. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000000&#x60;.
        /// </summary>
        /// <value>The maximum Web Transport payload size before fragmentation occurs for clients using the Client Profile, in bytes. The size of the header is not included. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000000&#x60;.</value>
        [DataMember(Name="serviceWebMaxPayload", EmitDefaultValue=false)]
        public long? ServiceWebMaxPayload { get; set; }

        /// <summary>
        /// The TCP initial congestion window size for clients using the Client Profile, in multiples of the TCP Maximum Segment Size (MSS). Changing the value from its default of 2 results in non-compliance with RFC 2581. Contact support before changing this value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;2&#x60;.
        /// </summary>
        /// <value>The TCP initial congestion window size for clients using the Client Profile, in multiples of the TCP Maximum Segment Size (MSS). Changing the value from its default of 2 results in non-compliance with RFC 2581. Contact support before changing this value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;2&#x60;.</value>
        [DataMember(Name="tcpCongestionWindowSize", EmitDefaultValue=false)]
        public long? TcpCongestionWindowSize { get; set; }

        /// <summary>
        /// The number of TCP keepalive retransmissions to a client using the Client Profile before declaring that it is not available. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;5&#x60;.
        /// </summary>
        /// <value>The number of TCP keepalive retransmissions to a client using the Client Profile before declaring that it is not available. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;5&#x60;.</value>
        [DataMember(Name="tcpKeepaliveCount", EmitDefaultValue=false)]
        public long? TcpKeepaliveCount { get; set; }

        /// <summary>
        /// The amount of time a client connection using the Client Profile must remain idle before TCP begins sending keepalive probes, in seconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;.
        /// </summary>
        /// <value>The amount of time a client connection using the Client Profile must remain idle before TCP begins sending keepalive probes, in seconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;.</value>
        [DataMember(Name="tcpKeepaliveIdleTime", EmitDefaultValue=false)]
        public long? TcpKeepaliveIdleTime { get; set; }

        /// <summary>
        /// The amount of time between TCP keepalive retransmissions to a client using the Client Profile when no acknowledgement is received, in seconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1&#x60;.
        /// </summary>
        /// <value>The amount of time between TCP keepalive retransmissions to a client using the Client Profile when no acknowledgement is received, in seconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1&#x60;.</value>
        [DataMember(Name="tcpKeepaliveInterval", EmitDefaultValue=false)]
        public long? TcpKeepaliveInterval { get; set; }

        /// <summary>
        /// The TCP maximum segment size for clients using the Client Profile, in bytes. Changes are applied to all existing connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1460&#x60;.
        /// </summary>
        /// <value>The TCP maximum segment size for clients using the Client Profile, in bytes. Changes are applied to all existing connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1460&#x60;.</value>
        [DataMember(Name="tcpMaxSegmentSize", EmitDefaultValue=false)]
        public long? TcpMaxSegmentSize { get; set; }

        /// <summary>
        /// The TCP maximum window size for clients using the Client Profile, in kilobytes. Changes are applied to all existing connections. This setting is ignored on the software broker. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;256&#x60;.
        /// </summary>
        /// <value>The TCP maximum window size for clients using the Client Profile, in kilobytes. Changes are applied to all existing connections. This setting is ignored on the software broker. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;256&#x60;.</value>
        [DataMember(Name="tcpMaxWindowSize", EmitDefaultValue=false)]
        public long? TcpMaxWindowSize { get; set; }

        /// <summary>
        /// Enable or disable allowing a client using the Client Profile to downgrade an encrypted connection to plain text. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;. Available since 2.8.
        /// </summary>
        /// <value>Enable or disable allowing a client using the Client Profile to downgrade an encrypted connection to plain text. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;. Available since 2.8.</value>
        [DataMember(Name="tlsAllowDowngradeToPlainTextEnabled", EmitDefaultValue=false)]
        public bool? TlsAllowDowngradeToPlainTextEnabled { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class MsgVpnClientProfile {\n");
            sb.Append("  AllowBridgeConnectionsEnabled: ").Append(AllowBridgeConnectionsEnabled).Append("\n");
            sb.Append("  AllowCutThroughForwardingEnabled: ").Append(AllowCutThroughForwardingEnabled).Append("\n");
            sb.Append("  AllowGuaranteedEndpointCreateDurability: ").Append(AllowGuaranteedEndpointCreateDurability).Append("\n");
            sb.Append("  AllowGuaranteedEndpointCreateEnabled: ").Append(AllowGuaranteedEndpointCreateEnabled).Append("\n");
            sb.Append("  AllowGuaranteedMsgReceiveEnabled: ").Append(AllowGuaranteedMsgReceiveEnabled).Append("\n");
            sb.Append("  AllowGuaranteedMsgSendEnabled: ").Append(AllowGuaranteedMsgSendEnabled).Append("\n");
            sb.Append("  AllowSharedSubscriptionsEnabled: ").Append(AllowSharedSubscriptionsEnabled).Append("\n");
            sb.Append("  AllowTransactedSessionsEnabled: ").Append(AllowTransactedSessionsEnabled).Append("\n");
            sb.Append("  ApiQueueManagementCopyFromOnCreateName: ").Append(ApiQueueManagementCopyFromOnCreateName).Append("\n");
            sb.Append("  ApiQueueManagementCopyFromOnCreateTemplateName: ").Append(ApiQueueManagementCopyFromOnCreateTemplateName).Append("\n");
            sb.Append("  ApiTopicEndpointManagementCopyFromOnCreateName: ").Append(ApiTopicEndpointManagementCopyFromOnCreateName).Append("\n");
            sb.Append("  ApiTopicEndpointManagementCopyFromOnCreateTemplateName: ").Append(ApiTopicEndpointManagementCopyFromOnCreateTemplateName).Append("\n");
            sb.Append("  ClientProfileName: ").Append(ClientProfileName).Append("\n");
            sb.Append("  CompressionEnabled: ").Append(CompressionEnabled).Append("\n");
            sb.Append("  ElidingDelay: ").Append(ElidingDelay).Append("\n");
            sb.Append("  ElidingEnabled: ").Append(ElidingEnabled).Append("\n");
            sb.Append("  ElidingMaxTopicCount: ").Append(ElidingMaxTopicCount).Append("\n");
            sb.Append("  EventClientProvisionedEndpointSpoolUsageThreshold: ").Append(EventClientProvisionedEndpointSpoolUsageThreshold).Append("\n");
            sb.Append("  EventConnectionCountPerClientUsernameThreshold: ").Append(EventConnectionCountPerClientUsernameThreshold).Append("\n");
            sb.Append("  EventEgressFlowCountThreshold: ").Append(EventEgressFlowCountThreshold).Append("\n");
            sb.Append("  EventEndpointCountPerClientUsernameThreshold: ").Append(EventEndpointCountPerClientUsernameThreshold).Append("\n");
            sb.Append("  EventIngressFlowCountThreshold: ").Append(EventIngressFlowCountThreshold).Append("\n");
            sb.Append("  EventServiceSmfConnectionCountPerClientUsernameThreshold: ").Append(EventServiceSmfConnectionCountPerClientUsernameThreshold).Append("\n");
            sb.Append("  EventServiceWebConnectionCountPerClientUsernameThreshold: ").Append(EventServiceWebConnectionCountPerClientUsernameThreshold).Append("\n");
            sb.Append("  EventSubscriptionCountThreshold: ").Append(EventSubscriptionCountThreshold).Append("\n");
            sb.Append("  EventTransactedSessionCountThreshold: ").Append(EventTransactedSessionCountThreshold).Append("\n");
            sb.Append("  EventTransactionCountThreshold: ").Append(EventTransactionCountThreshold).Append("\n");
            sb.Append("  MaxConnectionCountPerClientUsername: ").Append(MaxConnectionCountPerClientUsername).Append("\n");
            sb.Append("  MaxEgressFlowCount: ").Append(MaxEgressFlowCount).Append("\n");
            sb.Append("  MaxEndpointCountPerClientUsername: ").Append(MaxEndpointCountPerClientUsername).Append("\n");
            sb.Append("  MaxIngressFlowCount: ").Append(MaxIngressFlowCount).Append("\n");
            sb.Append("  MaxMsgsPerTransaction: ").Append(MaxMsgsPerTransaction).Append("\n");
            sb.Append("  MaxSubscriptionCount: ").Append(MaxSubscriptionCount).Append("\n");
            sb.Append("  MaxTransactedSessionCount: ").Append(MaxTransactedSessionCount).Append("\n");
            sb.Append("  MaxTransactionCount: ").Append(MaxTransactionCount).Append("\n");
            sb.Append("  MsgVpnName: ").Append(MsgVpnName).Append("\n");
            sb.Append("  QueueControl1MaxDepth: ").Append(QueueControl1MaxDepth).Append("\n");
            sb.Append("  QueueControl1MinMsgBurst: ").Append(QueueControl1MinMsgBurst).Append("\n");
            sb.Append("  QueueDirect1MaxDepth: ").Append(QueueDirect1MaxDepth).Append("\n");
            sb.Append("  QueueDirect1MinMsgBurst: ").Append(QueueDirect1MinMsgBurst).Append("\n");
            sb.Append("  QueueDirect2MaxDepth: ").Append(QueueDirect2MaxDepth).Append("\n");
            sb.Append("  QueueDirect2MinMsgBurst: ").Append(QueueDirect2MinMsgBurst).Append("\n");
            sb.Append("  QueueDirect3MaxDepth: ").Append(QueueDirect3MaxDepth).Append("\n");
            sb.Append("  QueueDirect3MinMsgBurst: ").Append(QueueDirect3MinMsgBurst).Append("\n");
            sb.Append("  QueueGuaranteed1MaxDepth: ").Append(QueueGuaranteed1MaxDepth).Append("\n");
            sb.Append("  QueueGuaranteed1MinMsgBurst: ").Append(QueueGuaranteed1MinMsgBurst).Append("\n");
            sb.Append("  RejectMsgToSenderOnNoSubscriptionMatchEnabled: ").Append(RejectMsgToSenderOnNoSubscriptionMatchEnabled).Append("\n");
            sb.Append("  ReplicationAllowClientConnectWhenStandbyEnabled: ").Append(ReplicationAllowClientConnectWhenStandbyEnabled).Append("\n");
            sb.Append("  ServiceMinKeepaliveTimeout: ").Append(ServiceMinKeepaliveTimeout).Append("\n");
            sb.Append("  ServiceSmfMaxConnectionCountPerClientUsername: ").Append(ServiceSmfMaxConnectionCountPerClientUsername).Append("\n");
            sb.Append("  ServiceSmfMinKeepaliveEnabled: ").Append(ServiceSmfMinKeepaliveEnabled).Append("\n");
            sb.Append("  ServiceWebInactiveTimeout: ").Append(ServiceWebInactiveTimeout).Append("\n");
            sb.Append("  ServiceWebMaxConnectionCountPerClientUsername: ").Append(ServiceWebMaxConnectionCountPerClientUsername).Append("\n");
            sb.Append("  ServiceWebMaxPayload: ").Append(ServiceWebMaxPayload).Append("\n");
            sb.Append("  TcpCongestionWindowSize: ").Append(TcpCongestionWindowSize).Append("\n");
            sb.Append("  TcpKeepaliveCount: ").Append(TcpKeepaliveCount).Append("\n");
            sb.Append("  TcpKeepaliveIdleTime: ").Append(TcpKeepaliveIdleTime).Append("\n");
            sb.Append("  TcpKeepaliveInterval: ").Append(TcpKeepaliveInterval).Append("\n");
            sb.Append("  TcpMaxSegmentSize: ").Append(TcpMaxSegmentSize).Append("\n");
            sb.Append("  TcpMaxWindowSize: ").Append(TcpMaxWindowSize).Append("\n");
            sb.Append("  TlsAllowDowngradeToPlainTextEnabled: ").Append(TlsAllowDowngradeToPlainTextEnabled).Append("\n");
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
            return this.Equals(input as MsgVpnClientProfile);
        }

        /// <summary>
        /// Returns true if MsgVpnClientProfile instances are equal
        /// </summary>
        /// <param name="input">Instance of MsgVpnClientProfile to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(MsgVpnClientProfile input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.AllowBridgeConnectionsEnabled == input.AllowBridgeConnectionsEnabled ||
                    (this.AllowBridgeConnectionsEnabled != null &&
                    this.AllowBridgeConnectionsEnabled.Equals(input.AllowBridgeConnectionsEnabled))
                ) && 
                (
                    this.AllowCutThroughForwardingEnabled == input.AllowCutThroughForwardingEnabled ||
                    (this.AllowCutThroughForwardingEnabled != null &&
                    this.AllowCutThroughForwardingEnabled.Equals(input.AllowCutThroughForwardingEnabled))
                ) && 
                (
                    this.AllowGuaranteedEndpointCreateDurability == input.AllowGuaranteedEndpointCreateDurability ||
                    (this.AllowGuaranteedEndpointCreateDurability != null &&
                    this.AllowGuaranteedEndpointCreateDurability.Equals(input.AllowGuaranteedEndpointCreateDurability))
                ) && 
                (
                    this.AllowGuaranteedEndpointCreateEnabled == input.AllowGuaranteedEndpointCreateEnabled ||
                    (this.AllowGuaranteedEndpointCreateEnabled != null &&
                    this.AllowGuaranteedEndpointCreateEnabled.Equals(input.AllowGuaranteedEndpointCreateEnabled))
                ) && 
                (
                    this.AllowGuaranteedMsgReceiveEnabled == input.AllowGuaranteedMsgReceiveEnabled ||
                    (this.AllowGuaranteedMsgReceiveEnabled != null &&
                    this.AllowGuaranteedMsgReceiveEnabled.Equals(input.AllowGuaranteedMsgReceiveEnabled))
                ) && 
                (
                    this.AllowGuaranteedMsgSendEnabled == input.AllowGuaranteedMsgSendEnabled ||
                    (this.AllowGuaranteedMsgSendEnabled != null &&
                    this.AllowGuaranteedMsgSendEnabled.Equals(input.AllowGuaranteedMsgSendEnabled))
                ) && 
                (
                    this.AllowSharedSubscriptionsEnabled == input.AllowSharedSubscriptionsEnabled ||
                    (this.AllowSharedSubscriptionsEnabled != null &&
                    this.AllowSharedSubscriptionsEnabled.Equals(input.AllowSharedSubscriptionsEnabled))
                ) && 
                (
                    this.AllowTransactedSessionsEnabled == input.AllowTransactedSessionsEnabled ||
                    (this.AllowTransactedSessionsEnabled != null &&
                    this.AllowTransactedSessionsEnabled.Equals(input.AllowTransactedSessionsEnabled))
                ) && 
                (
                    this.ApiQueueManagementCopyFromOnCreateName == input.ApiQueueManagementCopyFromOnCreateName ||
                    (this.ApiQueueManagementCopyFromOnCreateName != null &&
                    this.ApiQueueManagementCopyFromOnCreateName.Equals(input.ApiQueueManagementCopyFromOnCreateName))
                ) && 
                (
                    this.ApiQueueManagementCopyFromOnCreateTemplateName == input.ApiQueueManagementCopyFromOnCreateTemplateName ||
                    (this.ApiQueueManagementCopyFromOnCreateTemplateName != null &&
                    this.ApiQueueManagementCopyFromOnCreateTemplateName.Equals(input.ApiQueueManagementCopyFromOnCreateTemplateName))
                ) && 
                (
                    this.ApiTopicEndpointManagementCopyFromOnCreateName == input.ApiTopicEndpointManagementCopyFromOnCreateName ||
                    (this.ApiTopicEndpointManagementCopyFromOnCreateName != null &&
                    this.ApiTopicEndpointManagementCopyFromOnCreateName.Equals(input.ApiTopicEndpointManagementCopyFromOnCreateName))
                ) && 
                (
                    this.ApiTopicEndpointManagementCopyFromOnCreateTemplateName == input.ApiTopicEndpointManagementCopyFromOnCreateTemplateName ||
                    (this.ApiTopicEndpointManagementCopyFromOnCreateTemplateName != null &&
                    this.ApiTopicEndpointManagementCopyFromOnCreateTemplateName.Equals(input.ApiTopicEndpointManagementCopyFromOnCreateTemplateName))
                ) && 
                (
                    this.ClientProfileName == input.ClientProfileName ||
                    (this.ClientProfileName != null &&
                    this.ClientProfileName.Equals(input.ClientProfileName))
                ) && 
                (
                    this.CompressionEnabled == input.CompressionEnabled ||
                    (this.CompressionEnabled != null &&
                    this.CompressionEnabled.Equals(input.CompressionEnabled))
                ) && 
                (
                    this.ElidingDelay == input.ElidingDelay ||
                    (this.ElidingDelay != null &&
                    this.ElidingDelay.Equals(input.ElidingDelay))
                ) && 
                (
                    this.ElidingEnabled == input.ElidingEnabled ||
                    (this.ElidingEnabled != null &&
                    this.ElidingEnabled.Equals(input.ElidingEnabled))
                ) && 
                (
                    this.ElidingMaxTopicCount == input.ElidingMaxTopicCount ||
                    (this.ElidingMaxTopicCount != null &&
                    this.ElidingMaxTopicCount.Equals(input.ElidingMaxTopicCount))
                ) && 
                (
                    this.EventClientProvisionedEndpointSpoolUsageThreshold == input.EventClientProvisionedEndpointSpoolUsageThreshold ||
                    (this.EventClientProvisionedEndpointSpoolUsageThreshold != null &&
                    this.EventClientProvisionedEndpointSpoolUsageThreshold.Equals(input.EventClientProvisionedEndpointSpoolUsageThreshold))
                ) && 
                (
                    this.EventConnectionCountPerClientUsernameThreshold == input.EventConnectionCountPerClientUsernameThreshold ||
                    (this.EventConnectionCountPerClientUsernameThreshold != null &&
                    this.EventConnectionCountPerClientUsernameThreshold.Equals(input.EventConnectionCountPerClientUsernameThreshold))
                ) && 
                (
                    this.EventEgressFlowCountThreshold == input.EventEgressFlowCountThreshold ||
                    (this.EventEgressFlowCountThreshold != null &&
                    this.EventEgressFlowCountThreshold.Equals(input.EventEgressFlowCountThreshold))
                ) && 
                (
                    this.EventEndpointCountPerClientUsernameThreshold == input.EventEndpointCountPerClientUsernameThreshold ||
                    (this.EventEndpointCountPerClientUsernameThreshold != null &&
                    this.EventEndpointCountPerClientUsernameThreshold.Equals(input.EventEndpointCountPerClientUsernameThreshold))
                ) && 
                (
                    this.EventIngressFlowCountThreshold == input.EventIngressFlowCountThreshold ||
                    (this.EventIngressFlowCountThreshold != null &&
                    this.EventIngressFlowCountThreshold.Equals(input.EventIngressFlowCountThreshold))
                ) && 
                (
                    this.EventServiceSmfConnectionCountPerClientUsernameThreshold == input.EventServiceSmfConnectionCountPerClientUsernameThreshold ||
                    (this.EventServiceSmfConnectionCountPerClientUsernameThreshold != null &&
                    this.EventServiceSmfConnectionCountPerClientUsernameThreshold.Equals(input.EventServiceSmfConnectionCountPerClientUsernameThreshold))
                ) && 
                (
                    this.EventServiceWebConnectionCountPerClientUsernameThreshold == input.EventServiceWebConnectionCountPerClientUsernameThreshold ||
                    (this.EventServiceWebConnectionCountPerClientUsernameThreshold != null &&
                    this.EventServiceWebConnectionCountPerClientUsernameThreshold.Equals(input.EventServiceWebConnectionCountPerClientUsernameThreshold))
                ) && 
                (
                    this.EventSubscriptionCountThreshold == input.EventSubscriptionCountThreshold ||
                    (this.EventSubscriptionCountThreshold != null &&
                    this.EventSubscriptionCountThreshold.Equals(input.EventSubscriptionCountThreshold))
                ) && 
                (
                    this.EventTransactedSessionCountThreshold == input.EventTransactedSessionCountThreshold ||
                    (this.EventTransactedSessionCountThreshold != null &&
                    this.EventTransactedSessionCountThreshold.Equals(input.EventTransactedSessionCountThreshold))
                ) && 
                (
                    this.EventTransactionCountThreshold == input.EventTransactionCountThreshold ||
                    (this.EventTransactionCountThreshold != null &&
                    this.EventTransactionCountThreshold.Equals(input.EventTransactionCountThreshold))
                ) && 
                (
                    this.MaxConnectionCountPerClientUsername == input.MaxConnectionCountPerClientUsername ||
                    (this.MaxConnectionCountPerClientUsername != null &&
                    this.MaxConnectionCountPerClientUsername.Equals(input.MaxConnectionCountPerClientUsername))
                ) && 
                (
                    this.MaxEgressFlowCount == input.MaxEgressFlowCount ||
                    (this.MaxEgressFlowCount != null &&
                    this.MaxEgressFlowCount.Equals(input.MaxEgressFlowCount))
                ) && 
                (
                    this.MaxEndpointCountPerClientUsername == input.MaxEndpointCountPerClientUsername ||
                    (this.MaxEndpointCountPerClientUsername != null &&
                    this.MaxEndpointCountPerClientUsername.Equals(input.MaxEndpointCountPerClientUsername))
                ) && 
                (
                    this.MaxIngressFlowCount == input.MaxIngressFlowCount ||
                    (this.MaxIngressFlowCount != null &&
                    this.MaxIngressFlowCount.Equals(input.MaxIngressFlowCount))
                ) && 
                (
                    this.MaxMsgsPerTransaction == input.MaxMsgsPerTransaction ||
                    (this.MaxMsgsPerTransaction != null &&
                    this.MaxMsgsPerTransaction.Equals(input.MaxMsgsPerTransaction))
                ) && 
                (
                    this.MaxSubscriptionCount == input.MaxSubscriptionCount ||
                    (this.MaxSubscriptionCount != null &&
                    this.MaxSubscriptionCount.Equals(input.MaxSubscriptionCount))
                ) && 
                (
                    this.MaxTransactedSessionCount == input.MaxTransactedSessionCount ||
                    (this.MaxTransactedSessionCount != null &&
                    this.MaxTransactedSessionCount.Equals(input.MaxTransactedSessionCount))
                ) && 
                (
                    this.MaxTransactionCount == input.MaxTransactionCount ||
                    (this.MaxTransactionCount != null &&
                    this.MaxTransactionCount.Equals(input.MaxTransactionCount))
                ) && 
                (
                    this.MsgVpnName == input.MsgVpnName ||
                    (this.MsgVpnName != null &&
                    this.MsgVpnName.Equals(input.MsgVpnName))
                ) && 
                (
                    this.QueueControl1MaxDepth == input.QueueControl1MaxDepth ||
                    (this.QueueControl1MaxDepth != null &&
                    this.QueueControl1MaxDepth.Equals(input.QueueControl1MaxDepth))
                ) && 
                (
                    this.QueueControl1MinMsgBurst == input.QueueControl1MinMsgBurst ||
                    (this.QueueControl1MinMsgBurst != null &&
                    this.QueueControl1MinMsgBurst.Equals(input.QueueControl1MinMsgBurst))
                ) && 
                (
                    this.QueueDirect1MaxDepth == input.QueueDirect1MaxDepth ||
                    (this.QueueDirect1MaxDepth != null &&
                    this.QueueDirect1MaxDepth.Equals(input.QueueDirect1MaxDepth))
                ) && 
                (
                    this.QueueDirect1MinMsgBurst == input.QueueDirect1MinMsgBurst ||
                    (this.QueueDirect1MinMsgBurst != null &&
                    this.QueueDirect1MinMsgBurst.Equals(input.QueueDirect1MinMsgBurst))
                ) && 
                (
                    this.QueueDirect2MaxDepth == input.QueueDirect2MaxDepth ||
                    (this.QueueDirect2MaxDepth != null &&
                    this.QueueDirect2MaxDepth.Equals(input.QueueDirect2MaxDepth))
                ) && 
                (
                    this.QueueDirect2MinMsgBurst == input.QueueDirect2MinMsgBurst ||
                    (this.QueueDirect2MinMsgBurst != null &&
                    this.QueueDirect2MinMsgBurst.Equals(input.QueueDirect2MinMsgBurst))
                ) && 
                (
                    this.QueueDirect3MaxDepth == input.QueueDirect3MaxDepth ||
                    (this.QueueDirect3MaxDepth != null &&
                    this.QueueDirect3MaxDepth.Equals(input.QueueDirect3MaxDepth))
                ) && 
                (
                    this.QueueDirect3MinMsgBurst == input.QueueDirect3MinMsgBurst ||
                    (this.QueueDirect3MinMsgBurst != null &&
                    this.QueueDirect3MinMsgBurst.Equals(input.QueueDirect3MinMsgBurst))
                ) && 
                (
                    this.QueueGuaranteed1MaxDepth == input.QueueGuaranteed1MaxDepth ||
                    (this.QueueGuaranteed1MaxDepth != null &&
                    this.QueueGuaranteed1MaxDepth.Equals(input.QueueGuaranteed1MaxDepth))
                ) && 
                (
                    this.QueueGuaranteed1MinMsgBurst == input.QueueGuaranteed1MinMsgBurst ||
                    (this.QueueGuaranteed1MinMsgBurst != null &&
                    this.QueueGuaranteed1MinMsgBurst.Equals(input.QueueGuaranteed1MinMsgBurst))
                ) && 
                (
                    this.RejectMsgToSenderOnNoSubscriptionMatchEnabled == input.RejectMsgToSenderOnNoSubscriptionMatchEnabled ||
                    (this.RejectMsgToSenderOnNoSubscriptionMatchEnabled != null &&
                    this.RejectMsgToSenderOnNoSubscriptionMatchEnabled.Equals(input.RejectMsgToSenderOnNoSubscriptionMatchEnabled))
                ) && 
                (
                    this.ReplicationAllowClientConnectWhenStandbyEnabled == input.ReplicationAllowClientConnectWhenStandbyEnabled ||
                    (this.ReplicationAllowClientConnectWhenStandbyEnabled != null &&
                    this.ReplicationAllowClientConnectWhenStandbyEnabled.Equals(input.ReplicationAllowClientConnectWhenStandbyEnabled))
                ) && 
                (
                    this.ServiceMinKeepaliveTimeout == input.ServiceMinKeepaliveTimeout ||
                    (this.ServiceMinKeepaliveTimeout != null &&
                    this.ServiceMinKeepaliveTimeout.Equals(input.ServiceMinKeepaliveTimeout))
                ) && 
                (
                    this.ServiceSmfMaxConnectionCountPerClientUsername == input.ServiceSmfMaxConnectionCountPerClientUsername ||
                    (this.ServiceSmfMaxConnectionCountPerClientUsername != null &&
                    this.ServiceSmfMaxConnectionCountPerClientUsername.Equals(input.ServiceSmfMaxConnectionCountPerClientUsername))
                ) && 
                (
                    this.ServiceSmfMinKeepaliveEnabled == input.ServiceSmfMinKeepaliveEnabled ||
                    (this.ServiceSmfMinKeepaliveEnabled != null &&
                    this.ServiceSmfMinKeepaliveEnabled.Equals(input.ServiceSmfMinKeepaliveEnabled))
                ) && 
                (
                    this.ServiceWebInactiveTimeout == input.ServiceWebInactiveTimeout ||
                    (this.ServiceWebInactiveTimeout != null &&
                    this.ServiceWebInactiveTimeout.Equals(input.ServiceWebInactiveTimeout))
                ) && 
                (
                    this.ServiceWebMaxConnectionCountPerClientUsername == input.ServiceWebMaxConnectionCountPerClientUsername ||
                    (this.ServiceWebMaxConnectionCountPerClientUsername != null &&
                    this.ServiceWebMaxConnectionCountPerClientUsername.Equals(input.ServiceWebMaxConnectionCountPerClientUsername))
                ) && 
                (
                    this.ServiceWebMaxPayload == input.ServiceWebMaxPayload ||
                    (this.ServiceWebMaxPayload != null &&
                    this.ServiceWebMaxPayload.Equals(input.ServiceWebMaxPayload))
                ) && 
                (
                    this.TcpCongestionWindowSize == input.TcpCongestionWindowSize ||
                    (this.TcpCongestionWindowSize != null &&
                    this.TcpCongestionWindowSize.Equals(input.TcpCongestionWindowSize))
                ) && 
                (
                    this.TcpKeepaliveCount == input.TcpKeepaliveCount ||
                    (this.TcpKeepaliveCount != null &&
                    this.TcpKeepaliveCount.Equals(input.TcpKeepaliveCount))
                ) && 
                (
                    this.TcpKeepaliveIdleTime == input.TcpKeepaliveIdleTime ||
                    (this.TcpKeepaliveIdleTime != null &&
                    this.TcpKeepaliveIdleTime.Equals(input.TcpKeepaliveIdleTime))
                ) && 
                (
                    this.TcpKeepaliveInterval == input.TcpKeepaliveInterval ||
                    (this.TcpKeepaliveInterval != null &&
                    this.TcpKeepaliveInterval.Equals(input.TcpKeepaliveInterval))
                ) && 
                (
                    this.TcpMaxSegmentSize == input.TcpMaxSegmentSize ||
                    (this.TcpMaxSegmentSize != null &&
                    this.TcpMaxSegmentSize.Equals(input.TcpMaxSegmentSize))
                ) && 
                (
                    this.TcpMaxWindowSize == input.TcpMaxWindowSize ||
                    (this.TcpMaxWindowSize != null &&
                    this.TcpMaxWindowSize.Equals(input.TcpMaxWindowSize))
                ) && 
                (
                    this.TlsAllowDowngradeToPlainTextEnabled == input.TlsAllowDowngradeToPlainTextEnabled ||
                    (this.TlsAllowDowngradeToPlainTextEnabled != null &&
                    this.TlsAllowDowngradeToPlainTextEnabled.Equals(input.TlsAllowDowngradeToPlainTextEnabled))
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
                if (this.AllowBridgeConnectionsEnabled != null)
                    hashCode = hashCode * 59 + this.AllowBridgeConnectionsEnabled.GetHashCode();
                if (this.AllowCutThroughForwardingEnabled != null)
                    hashCode = hashCode * 59 + this.AllowCutThroughForwardingEnabled.GetHashCode();
                if (this.AllowGuaranteedEndpointCreateDurability != null)
                    hashCode = hashCode * 59 + this.AllowGuaranteedEndpointCreateDurability.GetHashCode();
                if (this.AllowGuaranteedEndpointCreateEnabled != null)
                    hashCode = hashCode * 59 + this.AllowGuaranteedEndpointCreateEnabled.GetHashCode();
                if (this.AllowGuaranteedMsgReceiveEnabled != null)
                    hashCode = hashCode * 59 + this.AllowGuaranteedMsgReceiveEnabled.GetHashCode();
                if (this.AllowGuaranteedMsgSendEnabled != null)
                    hashCode = hashCode * 59 + this.AllowGuaranteedMsgSendEnabled.GetHashCode();
                if (this.AllowSharedSubscriptionsEnabled != null)
                    hashCode = hashCode * 59 + this.AllowSharedSubscriptionsEnabled.GetHashCode();
                if (this.AllowTransactedSessionsEnabled != null)
                    hashCode = hashCode * 59 + this.AllowTransactedSessionsEnabled.GetHashCode();
                if (this.ApiQueueManagementCopyFromOnCreateName != null)
                    hashCode = hashCode * 59 + this.ApiQueueManagementCopyFromOnCreateName.GetHashCode();
                if (this.ApiQueueManagementCopyFromOnCreateTemplateName != null)
                    hashCode = hashCode * 59 + this.ApiQueueManagementCopyFromOnCreateTemplateName.GetHashCode();
                if (this.ApiTopicEndpointManagementCopyFromOnCreateName != null)
                    hashCode = hashCode * 59 + this.ApiTopicEndpointManagementCopyFromOnCreateName.GetHashCode();
                if (this.ApiTopicEndpointManagementCopyFromOnCreateTemplateName != null)
                    hashCode = hashCode * 59 + this.ApiTopicEndpointManagementCopyFromOnCreateTemplateName.GetHashCode();
                if (this.ClientProfileName != null)
                    hashCode = hashCode * 59 + this.ClientProfileName.GetHashCode();
                if (this.CompressionEnabled != null)
                    hashCode = hashCode * 59 + this.CompressionEnabled.GetHashCode();
                if (this.ElidingDelay != null)
                    hashCode = hashCode * 59 + this.ElidingDelay.GetHashCode();
                if (this.ElidingEnabled != null)
                    hashCode = hashCode * 59 + this.ElidingEnabled.GetHashCode();
                if (this.ElidingMaxTopicCount != null)
                    hashCode = hashCode * 59 + this.ElidingMaxTopicCount.GetHashCode();
                if (this.EventClientProvisionedEndpointSpoolUsageThreshold != null)
                    hashCode = hashCode * 59 + this.EventClientProvisionedEndpointSpoolUsageThreshold.GetHashCode();
                if (this.EventConnectionCountPerClientUsernameThreshold != null)
                    hashCode = hashCode * 59 + this.EventConnectionCountPerClientUsernameThreshold.GetHashCode();
                if (this.EventEgressFlowCountThreshold != null)
                    hashCode = hashCode * 59 + this.EventEgressFlowCountThreshold.GetHashCode();
                if (this.EventEndpointCountPerClientUsernameThreshold != null)
                    hashCode = hashCode * 59 + this.EventEndpointCountPerClientUsernameThreshold.GetHashCode();
                if (this.EventIngressFlowCountThreshold != null)
                    hashCode = hashCode * 59 + this.EventIngressFlowCountThreshold.GetHashCode();
                if (this.EventServiceSmfConnectionCountPerClientUsernameThreshold != null)
                    hashCode = hashCode * 59 + this.EventServiceSmfConnectionCountPerClientUsernameThreshold.GetHashCode();
                if (this.EventServiceWebConnectionCountPerClientUsernameThreshold != null)
                    hashCode = hashCode * 59 + this.EventServiceWebConnectionCountPerClientUsernameThreshold.GetHashCode();
                if (this.EventSubscriptionCountThreshold != null)
                    hashCode = hashCode * 59 + this.EventSubscriptionCountThreshold.GetHashCode();
                if (this.EventTransactedSessionCountThreshold != null)
                    hashCode = hashCode * 59 + this.EventTransactedSessionCountThreshold.GetHashCode();
                if (this.EventTransactionCountThreshold != null)
                    hashCode = hashCode * 59 + this.EventTransactionCountThreshold.GetHashCode();
                if (this.MaxConnectionCountPerClientUsername != null)
                    hashCode = hashCode * 59 + this.MaxConnectionCountPerClientUsername.GetHashCode();
                if (this.MaxEgressFlowCount != null)
                    hashCode = hashCode * 59 + this.MaxEgressFlowCount.GetHashCode();
                if (this.MaxEndpointCountPerClientUsername != null)
                    hashCode = hashCode * 59 + this.MaxEndpointCountPerClientUsername.GetHashCode();
                if (this.MaxIngressFlowCount != null)
                    hashCode = hashCode * 59 + this.MaxIngressFlowCount.GetHashCode();
                if (this.MaxMsgsPerTransaction != null)
                    hashCode = hashCode * 59 + this.MaxMsgsPerTransaction.GetHashCode();
                if (this.MaxSubscriptionCount != null)
                    hashCode = hashCode * 59 + this.MaxSubscriptionCount.GetHashCode();
                if (this.MaxTransactedSessionCount != null)
                    hashCode = hashCode * 59 + this.MaxTransactedSessionCount.GetHashCode();
                if (this.MaxTransactionCount != null)
                    hashCode = hashCode * 59 + this.MaxTransactionCount.GetHashCode();
                if (this.MsgVpnName != null)
                    hashCode = hashCode * 59 + this.MsgVpnName.GetHashCode();
                if (this.QueueControl1MaxDepth != null)
                    hashCode = hashCode * 59 + this.QueueControl1MaxDepth.GetHashCode();
                if (this.QueueControl1MinMsgBurst != null)
                    hashCode = hashCode * 59 + this.QueueControl1MinMsgBurst.GetHashCode();
                if (this.QueueDirect1MaxDepth != null)
                    hashCode = hashCode * 59 + this.QueueDirect1MaxDepth.GetHashCode();
                if (this.QueueDirect1MinMsgBurst != null)
                    hashCode = hashCode * 59 + this.QueueDirect1MinMsgBurst.GetHashCode();
                if (this.QueueDirect2MaxDepth != null)
                    hashCode = hashCode * 59 + this.QueueDirect2MaxDepth.GetHashCode();
                if (this.QueueDirect2MinMsgBurst != null)
                    hashCode = hashCode * 59 + this.QueueDirect2MinMsgBurst.GetHashCode();
                if (this.QueueDirect3MaxDepth != null)
                    hashCode = hashCode * 59 + this.QueueDirect3MaxDepth.GetHashCode();
                if (this.QueueDirect3MinMsgBurst != null)
                    hashCode = hashCode * 59 + this.QueueDirect3MinMsgBurst.GetHashCode();
                if (this.QueueGuaranteed1MaxDepth != null)
                    hashCode = hashCode * 59 + this.QueueGuaranteed1MaxDepth.GetHashCode();
                if (this.QueueGuaranteed1MinMsgBurst != null)
                    hashCode = hashCode * 59 + this.QueueGuaranteed1MinMsgBurst.GetHashCode();
                if (this.RejectMsgToSenderOnNoSubscriptionMatchEnabled != null)
                    hashCode = hashCode * 59 + this.RejectMsgToSenderOnNoSubscriptionMatchEnabled.GetHashCode();
                if (this.ReplicationAllowClientConnectWhenStandbyEnabled != null)
                    hashCode = hashCode * 59 + this.ReplicationAllowClientConnectWhenStandbyEnabled.GetHashCode();
                if (this.ServiceMinKeepaliveTimeout != null)
                    hashCode = hashCode * 59 + this.ServiceMinKeepaliveTimeout.GetHashCode();
                if (this.ServiceSmfMaxConnectionCountPerClientUsername != null)
                    hashCode = hashCode * 59 + this.ServiceSmfMaxConnectionCountPerClientUsername.GetHashCode();
                if (this.ServiceSmfMinKeepaliveEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceSmfMinKeepaliveEnabled.GetHashCode();
                if (this.ServiceWebInactiveTimeout != null)
                    hashCode = hashCode * 59 + this.ServiceWebInactiveTimeout.GetHashCode();
                if (this.ServiceWebMaxConnectionCountPerClientUsername != null)
                    hashCode = hashCode * 59 + this.ServiceWebMaxConnectionCountPerClientUsername.GetHashCode();
                if (this.ServiceWebMaxPayload != null)
                    hashCode = hashCode * 59 + this.ServiceWebMaxPayload.GetHashCode();
                if (this.TcpCongestionWindowSize != null)
                    hashCode = hashCode * 59 + this.TcpCongestionWindowSize.GetHashCode();
                if (this.TcpKeepaliveCount != null)
                    hashCode = hashCode * 59 + this.TcpKeepaliveCount.GetHashCode();
                if (this.TcpKeepaliveIdleTime != null)
                    hashCode = hashCode * 59 + this.TcpKeepaliveIdleTime.GetHashCode();
                if (this.TcpKeepaliveInterval != null)
                    hashCode = hashCode * 59 + this.TcpKeepaliveInterval.GetHashCode();
                if (this.TcpMaxSegmentSize != null)
                    hashCode = hashCode * 59 + this.TcpMaxSegmentSize.GetHashCode();
                if (this.TcpMaxWindowSize != null)
                    hashCode = hashCode * 59 + this.TcpMaxWindowSize.GetHashCode();
                if (this.TlsAllowDowngradeToPlainTextEnabled != null)
                    hashCode = hashCode * 59 + this.TlsAllowDowngradeToPlainTextEnabled.GetHashCode();
                return hashCode;
            }
        }
    }
}
