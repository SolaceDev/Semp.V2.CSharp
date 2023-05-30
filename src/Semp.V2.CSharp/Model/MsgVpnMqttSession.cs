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
    /// MsgVpnMqttSession
    /// </summary>
    [DataContract]
        public partial class MsgVpnMqttSession :  IEquatable<MsgVpnMqttSession>
    {
        /// <summary>
        /// The virtual router of the MQTT Session. The allowed values and their meaning are:  &lt;pre&gt; \&quot;primary\&quot; - The MQTT Session belongs to the primary virtual router. \&quot;backup\&quot; - The MQTT Session belongs to the backup virtual router. \&quot;auto\&quot; - The MQTT Session is automatically assigned a virtual router at creation, depending on the broker&#x27;s active-standby role. &lt;/pre&gt; 
        /// </summary>
        /// <value>The virtual router of the MQTT Session. The allowed values and their meaning are:  &lt;pre&gt; \&quot;primary\&quot; - The MQTT Session belongs to the primary virtual router. \&quot;backup\&quot; - The MQTT Session belongs to the backup virtual router. \&quot;auto\&quot; - The MQTT Session is automatically assigned a virtual router at creation, depending on the broker&#x27;s active-standby role. &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum MqttSessionVirtualRouterEnum
        {
            /// <summary>
            /// Enum Primary for value: primary
            /// </summary>
            [EnumMember(Value = "primary")]
            Primary = 1,
            /// <summary>
            /// Enum Backup for value: backup
            /// </summary>
            [EnumMember(Value = "backup")]
            Backup = 2,
            /// <summary>
            /// Enum Auto for value: auto
            /// </summary>
            [EnumMember(Value = "auto")]
            Auto = 3        }
        /// <summary>
        /// The virtual router of the MQTT Session. The allowed values and their meaning are:  &lt;pre&gt; \&quot;primary\&quot; - The MQTT Session belongs to the primary virtual router. \&quot;backup\&quot; - The MQTT Session belongs to the backup virtual router. \&quot;auto\&quot; - The MQTT Session is automatically assigned a virtual router at creation, depending on the broker&#x27;s active-standby role. &lt;/pre&gt; 
        /// </summary>
        /// <value>The virtual router of the MQTT Session. The allowed values and their meaning are:  &lt;pre&gt; \&quot;primary\&quot; - The MQTT Session belongs to the primary virtual router. \&quot;backup\&quot; - The MQTT Session belongs to the backup virtual router. \&quot;auto\&quot; - The MQTT Session is automatically assigned a virtual router at creation, depending on the broker&#x27;s active-standby role. &lt;/pre&gt; </value>
        [DataMember(Name="mqttSessionVirtualRouter", EmitDefaultValue=false)]
        public MqttSessionVirtualRouterEnum? MqttSessionVirtualRouter { get; set; }
        /// <summary>
        /// Determines when to return negative acknowledgements (NACKs) to sending clients on message discards. Note that NACKs cause the message to not be delivered to any destination and Transacted Session commits to fail. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as queueRejectLowPriorityMsgEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-queue-enabled\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always return a negative acknowledgment (NACK) to the sending client on message discard. \&quot;when-queue-enabled\&quot; - Only return a negative acknowledgment (NACK) to the sending client on message discard when the Queue is enabled. \&quot;never\&quot; - Never return a negative acknowledgment (NACK) to the sending client on message discard. &lt;/pre&gt;  Available since 2.14.
        /// </summary>
        /// <value>Determines when to return negative acknowledgements (NACKs) to sending clients on message discards. Note that NACKs cause the message to not be delivered to any destination and Transacted Session commits to fail. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as queueRejectLowPriorityMsgEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-queue-enabled\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always return a negative acknowledgment (NACK) to the sending client on message discard. \&quot;when-queue-enabled\&quot; - Only return a negative acknowledgment (NACK) to the sending client on message discard when the Queue is enabled. \&quot;never\&quot; - Never return a negative acknowledgment (NACK) to the sending client on message discard. &lt;/pre&gt;  Available since 2.14.</value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum QueueRejectMsgToSenderOnDiscardBehaviorEnum
        {
            /// <summary>
            /// Enum Always for value: always
            /// </summary>
            [EnumMember(Value = "always")]
            Always = 1,
            /// <summary>
            /// Enum WhenQueueEnabled for value: when-queue-enabled
            /// </summary>
            [EnumMember(Value = "when-queue-enabled")]
            WhenQueueEnabled = 2,
            /// <summary>
            /// Enum Never for value: never
            /// </summary>
            [EnumMember(Value = "never")]
            Never = 3        }
        /// <summary>
        /// Determines when to return negative acknowledgements (NACKs) to sending clients on message discards. Note that NACKs cause the message to not be delivered to any destination and Transacted Session commits to fail. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as queueRejectLowPriorityMsgEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-queue-enabled\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always return a negative acknowledgment (NACK) to the sending client on message discard. \&quot;when-queue-enabled\&quot; - Only return a negative acknowledgment (NACK) to the sending client on message discard when the Queue is enabled. \&quot;never\&quot; - Never return a negative acknowledgment (NACK) to the sending client on message discard. &lt;/pre&gt;  Available since 2.14.
        /// </summary>
        /// <value>Determines when to return negative acknowledgements (NACKs) to sending clients on message discards. Note that NACKs cause the message to not be delivered to any destination and Transacted Session commits to fail. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as queueRejectLowPriorityMsgEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-queue-enabled\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always return a negative acknowledgment (NACK) to the sending client on message discard. \&quot;when-queue-enabled\&quot; - Only return a negative acknowledgment (NACK) to the sending client on message discard when the Queue is enabled. \&quot;never\&quot; - Never return a negative acknowledgment (NACK) to the sending client on message discard. &lt;/pre&gt;  Available since 2.14.</value>
        [DataMember(Name="queueRejectMsgToSenderOnDiscardBehavior", EmitDefaultValue=false)]
        public QueueRejectMsgToSenderOnDiscardBehaviorEnum? QueueRejectMsgToSenderOnDiscardBehavior { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="MsgVpnMqttSession" /> class.
        /// </summary>
        /// <param name="enabled">Enable or disable the MQTT Session. When disabled, the client is disconnected, new messages matching QoS 0 subscriptions are discarded, and new messages matching QoS 1 subscriptions are stored for future delivery. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="mqttSessionClientId">The Client ID of the MQTT Session, which corresponds to the ClientId provided in the MQTT CONNECT packet..</param>
        /// <param name="mqttSessionVirtualRouter">The virtual router of the MQTT Session. The allowed values and their meaning are:  &lt;pre&gt; \&quot;primary\&quot; - The MQTT Session belongs to the primary virtual router. \&quot;backup\&quot; - The MQTT Session belongs to the backup virtual router. \&quot;auto\&quot; - The MQTT Session is automatically assigned a virtual router at creation, depending on the broker&#x27;s active-standby role. &lt;/pre&gt; .</param>
        /// <param name="msgVpnName">The name of the Message VPN..</param>
        /// <param name="owner">The owner of the MQTT Session. For externally-created sessions this defaults to the Client Username of the connecting client. For management-created sessions this defaults to empty. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="queueConsumerAckPropagationEnabled">Enable or disable the propagation of consumer acknowledgements (ACKs) received on the active replication Message VPN to the standby replication Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;. Available since 2.14..</param>
        /// <param name="queueDeadMsgQueue">The name of the Dead Message Queue (DMQ) used by the MQTT Session Queue. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;#DEAD_MSG_QUEUE\&quot;&#x60;. Available since 2.14..</param>
        /// <param name="queueEventBindCountThreshold">queueEventBindCountThreshold.</param>
        /// <param name="queueEventMsgSpoolUsageThreshold">queueEventMsgSpoolUsageThreshold.</param>
        /// <param name="queueEventRejectLowPriorityMsgLimitThreshold">queueEventRejectLowPriorityMsgLimitThreshold.</param>
        /// <param name="queueMaxBindCount">The maximum number of consumer flows that can bind to the MQTT Session Queue. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;. Available since 2.14..</param>
        /// <param name="queueMaxDeliveredUnackedMsgsPerFlow">The maximum number of messages delivered but not acknowledged per flow for the MQTT Session Queue. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;10000&#x60;. Available since 2.14..</param>
        /// <param name="queueMaxMsgSize">The maximum message size allowed in the MQTT Session Queue, in bytes (B). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;10000000&#x60;. Available since 2.14..</param>
        /// <param name="queueMaxMsgSpoolUsage">The maximum message spool usage allowed by the MQTT Session Queue, in megabytes (MB). A value of 0 only allows spooling of the last message received and disables quota checking. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;5000&#x60;. Available since 2.14..</param>
        /// <param name="queueMaxRedeliveryCount">The maximum number of times the MQTT Session Queue will attempt redelivery of a message prior to it being discarded or moved to the DMQ. A value of 0 means to retry forever. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.14..</param>
        /// <param name="queueMaxTtl">The maximum time in seconds a message can stay in the MQTT Session Queue when &#x60;queueRespectTtlEnabled&#x60; is &#x60;\&quot;true\&quot;&#x60;. A message expires when the lesser of the sender assigned time-to-live (TTL) in the message and the &#x60;queueMaxTtl&#x60; configured for the MQTT Session Queue, is exceeded. A value of 0 disables expiry. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.14..</param>
        /// <param name="queueRejectLowPriorityMsgEnabled">Enable or disable the checking of low priority messages against the &#x60;queueRejectLowPriorityMsgLimit&#x60;. This may only be enabled if &#x60;queueRejectMsgToSenderOnDiscardBehavior&#x60; does not have a value of &#x60;\&quot;never\&quot;&#x60;. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.14..</param>
        /// <param name="queueRejectLowPriorityMsgLimit">The number of messages of any priority in the MQTT Session Queue above which low priority messages are not admitted but higher priority messages are allowed. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.14..</param>
        /// <param name="queueRejectMsgToSenderOnDiscardBehavior">Determines when to return negative acknowledgements (NACKs) to sending clients on message discards. Note that NACKs cause the message to not be delivered to any destination and Transacted Session commits to fail. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as queueRejectLowPriorityMsgEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-queue-enabled\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always return a negative acknowledgment (NACK) to the sending client on message discard. \&quot;when-queue-enabled\&quot; - Only return a negative acknowledgment (NACK) to the sending client on message discard when the Queue is enabled. \&quot;never\&quot; - Never return a negative acknowledgment (NACK) to the sending client on message discard. &lt;/pre&gt;  Available since 2.14..</param>
        /// <param name="queueRespectTtlEnabled">Enable or disable the respecting of the time-to-live (TTL) for messages in the MQTT Session Queue. When enabled, expired messages are discarded or moved to the DMQ. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.14..</param>
        public MsgVpnMqttSession(bool? enabled = default(bool?), string mqttSessionClientId = default(string), MqttSessionVirtualRouterEnum? mqttSessionVirtualRouter = default(MqttSessionVirtualRouterEnum?), string msgVpnName = default(string), string owner = default(string), bool? queueConsumerAckPropagationEnabled = default(bool?), string queueDeadMsgQueue = default(string), EventThreshold queueEventBindCountThreshold = default(EventThreshold), EventThreshold queueEventMsgSpoolUsageThreshold = default(EventThreshold), EventThreshold queueEventRejectLowPriorityMsgLimitThreshold = default(EventThreshold), long? queueMaxBindCount = default(long?), long? queueMaxDeliveredUnackedMsgsPerFlow = default(long?), int? queueMaxMsgSize = default(int?), long? queueMaxMsgSpoolUsage = default(long?), long? queueMaxRedeliveryCount = default(long?), long? queueMaxTtl = default(long?), bool? queueRejectLowPriorityMsgEnabled = default(bool?), long? queueRejectLowPriorityMsgLimit = default(long?), QueueRejectMsgToSenderOnDiscardBehaviorEnum? queueRejectMsgToSenderOnDiscardBehavior = default(QueueRejectMsgToSenderOnDiscardBehaviorEnum?), bool? queueRespectTtlEnabled = default(bool?))
        {
            this.Enabled = enabled;
            this.MqttSessionClientId = mqttSessionClientId;
            this.MqttSessionVirtualRouter = mqttSessionVirtualRouter;
            this.MsgVpnName = msgVpnName;
            this.Owner = owner;
            this.QueueConsumerAckPropagationEnabled = queueConsumerAckPropagationEnabled;
            this.QueueDeadMsgQueue = queueDeadMsgQueue;
            this.QueueEventBindCountThreshold = queueEventBindCountThreshold;
            this.QueueEventMsgSpoolUsageThreshold = queueEventMsgSpoolUsageThreshold;
            this.QueueEventRejectLowPriorityMsgLimitThreshold = queueEventRejectLowPriorityMsgLimitThreshold;
            this.QueueMaxBindCount = queueMaxBindCount;
            this.QueueMaxDeliveredUnackedMsgsPerFlow = queueMaxDeliveredUnackedMsgsPerFlow;
            this.QueueMaxMsgSize = queueMaxMsgSize;
            this.QueueMaxMsgSpoolUsage = queueMaxMsgSpoolUsage;
            this.QueueMaxRedeliveryCount = queueMaxRedeliveryCount;
            this.QueueMaxTtl = queueMaxTtl;
            this.QueueRejectLowPriorityMsgEnabled = queueRejectLowPriorityMsgEnabled;
            this.QueueRejectLowPriorityMsgLimit = queueRejectLowPriorityMsgLimit;
            this.QueueRejectMsgToSenderOnDiscardBehavior = queueRejectMsgToSenderOnDiscardBehavior;
            this.QueueRespectTtlEnabled = queueRespectTtlEnabled;
        }
        
        /// <summary>
        /// Enable or disable the MQTT Session. When disabled, the client is disconnected, new messages matching QoS 0 subscriptions are discarded, and new messages matching QoS 1 subscriptions are stored for future delivery. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable the MQTT Session. When disabled, the client is disconnected, new messages matching QoS 0 subscriptions are discarded, and new messages matching QoS 1 subscriptions are stored for future delivery. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="enabled", EmitDefaultValue=false)]
        public bool? Enabled { get; set; }

        /// <summary>
        /// The Client ID of the MQTT Session, which corresponds to the ClientId provided in the MQTT CONNECT packet.
        /// </summary>
        /// <value>The Client ID of the MQTT Session, which corresponds to the ClientId provided in the MQTT CONNECT packet.</value>
        [DataMember(Name="mqttSessionClientId", EmitDefaultValue=false)]
        public string MqttSessionClientId { get; set; }


        /// <summary>
        /// The name of the Message VPN.
        /// </summary>
        /// <value>The name of the Message VPN.</value>
        [DataMember(Name="msgVpnName", EmitDefaultValue=false)]
        public string MsgVpnName { get; set; }

        /// <summary>
        /// The owner of the MQTT Session. For externally-created sessions this defaults to the Client Username of the connecting client. For management-created sessions this defaults to empty. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The owner of the MQTT Session. For externally-created sessions this defaults to the Client Username of the connecting client. For management-created sessions this defaults to empty. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="owner", EmitDefaultValue=false)]
        public string Owner { get; set; }

        /// <summary>
        /// Enable or disable the propagation of consumer acknowledgements (ACKs) received on the active replication Message VPN to the standby replication Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;. Available since 2.14.
        /// </summary>
        /// <value>Enable or disable the propagation of consumer acknowledgements (ACKs) received on the active replication Message VPN to the standby replication Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;. Available since 2.14.</value>
        [DataMember(Name="queueConsumerAckPropagationEnabled", EmitDefaultValue=false)]
        public bool? QueueConsumerAckPropagationEnabled { get; set; }

        /// <summary>
        /// The name of the Dead Message Queue (DMQ) used by the MQTT Session Queue. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;#DEAD_MSG_QUEUE\&quot;&#x60;. Available since 2.14.
        /// </summary>
        /// <value>The name of the Dead Message Queue (DMQ) used by the MQTT Session Queue. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;#DEAD_MSG_QUEUE\&quot;&#x60;. Available since 2.14.</value>
        [DataMember(Name="queueDeadMsgQueue", EmitDefaultValue=false)]
        public string QueueDeadMsgQueue { get; set; }

        /// <summary>
        /// Gets or Sets QueueEventBindCountThreshold
        /// </summary>
        [DataMember(Name="queueEventBindCountThreshold", EmitDefaultValue=false)]
        public EventThreshold QueueEventBindCountThreshold { get; set; }

        /// <summary>
        /// Gets or Sets QueueEventMsgSpoolUsageThreshold
        /// </summary>
        [DataMember(Name="queueEventMsgSpoolUsageThreshold", EmitDefaultValue=false)]
        public EventThreshold QueueEventMsgSpoolUsageThreshold { get; set; }

        /// <summary>
        /// Gets or Sets QueueEventRejectLowPriorityMsgLimitThreshold
        /// </summary>
        [DataMember(Name="queueEventRejectLowPriorityMsgLimitThreshold", EmitDefaultValue=false)]
        public EventThreshold QueueEventRejectLowPriorityMsgLimitThreshold { get; set; }

        /// <summary>
        /// The maximum number of consumer flows that can bind to the MQTT Session Queue. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;. Available since 2.14.
        /// </summary>
        /// <value>The maximum number of consumer flows that can bind to the MQTT Session Queue. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;. Available since 2.14.</value>
        [DataMember(Name="queueMaxBindCount", EmitDefaultValue=false)]
        public long? QueueMaxBindCount { get; set; }

        /// <summary>
        /// The maximum number of messages delivered but not acknowledged per flow for the MQTT Session Queue. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;10000&#x60;. Available since 2.14.
        /// </summary>
        /// <value>The maximum number of messages delivered but not acknowledged per flow for the MQTT Session Queue. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;10000&#x60;. Available since 2.14.</value>
        [DataMember(Name="queueMaxDeliveredUnackedMsgsPerFlow", EmitDefaultValue=false)]
        public long? QueueMaxDeliveredUnackedMsgsPerFlow { get; set; }

        /// <summary>
        /// The maximum message size allowed in the MQTT Session Queue, in bytes (B). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;10000000&#x60;. Available since 2.14.
        /// </summary>
        /// <value>The maximum message size allowed in the MQTT Session Queue, in bytes (B). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;10000000&#x60;. Available since 2.14.</value>
        [DataMember(Name="queueMaxMsgSize", EmitDefaultValue=false)]
        public int? QueueMaxMsgSize { get; set; }

        /// <summary>
        /// The maximum message spool usage allowed by the MQTT Session Queue, in megabytes (MB). A value of 0 only allows spooling of the last message received and disables quota checking. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;5000&#x60;. Available since 2.14.
        /// </summary>
        /// <value>The maximum message spool usage allowed by the MQTT Session Queue, in megabytes (MB). A value of 0 only allows spooling of the last message received and disables quota checking. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;5000&#x60;. Available since 2.14.</value>
        [DataMember(Name="queueMaxMsgSpoolUsage", EmitDefaultValue=false)]
        public long? QueueMaxMsgSpoolUsage { get; set; }

        /// <summary>
        /// The maximum number of times the MQTT Session Queue will attempt redelivery of a message prior to it being discarded or moved to the DMQ. A value of 0 means to retry forever. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.14.
        /// </summary>
        /// <value>The maximum number of times the MQTT Session Queue will attempt redelivery of a message prior to it being discarded or moved to the DMQ. A value of 0 means to retry forever. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.14.</value>
        [DataMember(Name="queueMaxRedeliveryCount", EmitDefaultValue=false)]
        public long? QueueMaxRedeliveryCount { get; set; }

        /// <summary>
        /// The maximum time in seconds a message can stay in the MQTT Session Queue when &#x60;queueRespectTtlEnabled&#x60; is &#x60;\&quot;true\&quot;&#x60;. A message expires when the lesser of the sender assigned time-to-live (TTL) in the message and the &#x60;queueMaxTtl&#x60; configured for the MQTT Session Queue, is exceeded. A value of 0 disables expiry. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.14.
        /// </summary>
        /// <value>The maximum time in seconds a message can stay in the MQTT Session Queue when &#x60;queueRespectTtlEnabled&#x60; is &#x60;\&quot;true\&quot;&#x60;. A message expires when the lesser of the sender assigned time-to-live (TTL) in the message and the &#x60;queueMaxTtl&#x60; configured for the MQTT Session Queue, is exceeded. A value of 0 disables expiry. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.14.</value>
        [DataMember(Name="queueMaxTtl", EmitDefaultValue=false)]
        public long? QueueMaxTtl { get; set; }

        /// <summary>
        /// Enable or disable the checking of low priority messages against the &#x60;queueRejectLowPriorityMsgLimit&#x60;. This may only be enabled if &#x60;queueRejectMsgToSenderOnDiscardBehavior&#x60; does not have a value of &#x60;\&quot;never\&quot;&#x60;. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.14.
        /// </summary>
        /// <value>Enable or disable the checking of low priority messages against the &#x60;queueRejectLowPriorityMsgLimit&#x60;. This may only be enabled if &#x60;queueRejectMsgToSenderOnDiscardBehavior&#x60; does not have a value of &#x60;\&quot;never\&quot;&#x60;. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.14.</value>
        [DataMember(Name="queueRejectLowPriorityMsgEnabled", EmitDefaultValue=false)]
        public bool? QueueRejectLowPriorityMsgEnabled { get; set; }

        /// <summary>
        /// The number of messages of any priority in the MQTT Session Queue above which low priority messages are not admitted but higher priority messages are allowed. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.14.
        /// </summary>
        /// <value>The number of messages of any priority in the MQTT Session Queue above which low priority messages are not admitted but higher priority messages are allowed. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.14.</value>
        [DataMember(Name="queueRejectLowPriorityMsgLimit", EmitDefaultValue=false)]
        public long? QueueRejectLowPriorityMsgLimit { get; set; }


        /// <summary>
        /// Enable or disable the respecting of the time-to-live (TTL) for messages in the MQTT Session Queue. When enabled, expired messages are discarded or moved to the DMQ. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.14.
        /// </summary>
        /// <value>Enable or disable the respecting of the time-to-live (TTL) for messages in the MQTT Session Queue. When enabled, expired messages are discarded or moved to the DMQ. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.14.</value>
        [DataMember(Name="queueRespectTtlEnabled", EmitDefaultValue=false)]
        public bool? QueueRespectTtlEnabled { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class MsgVpnMqttSession {\n");
            sb.Append("  Enabled: ").Append(Enabled).Append("\n");
            sb.Append("  MqttSessionClientId: ").Append(MqttSessionClientId).Append("\n");
            sb.Append("  MqttSessionVirtualRouter: ").Append(MqttSessionVirtualRouter).Append("\n");
            sb.Append("  MsgVpnName: ").Append(MsgVpnName).Append("\n");
            sb.Append("  Owner: ").Append(Owner).Append("\n");
            sb.Append("  QueueConsumerAckPropagationEnabled: ").Append(QueueConsumerAckPropagationEnabled).Append("\n");
            sb.Append("  QueueDeadMsgQueue: ").Append(QueueDeadMsgQueue).Append("\n");
            sb.Append("  QueueEventBindCountThreshold: ").Append(QueueEventBindCountThreshold).Append("\n");
            sb.Append("  QueueEventMsgSpoolUsageThreshold: ").Append(QueueEventMsgSpoolUsageThreshold).Append("\n");
            sb.Append("  QueueEventRejectLowPriorityMsgLimitThreshold: ").Append(QueueEventRejectLowPriorityMsgLimitThreshold).Append("\n");
            sb.Append("  QueueMaxBindCount: ").Append(QueueMaxBindCount).Append("\n");
            sb.Append("  QueueMaxDeliveredUnackedMsgsPerFlow: ").Append(QueueMaxDeliveredUnackedMsgsPerFlow).Append("\n");
            sb.Append("  QueueMaxMsgSize: ").Append(QueueMaxMsgSize).Append("\n");
            sb.Append("  QueueMaxMsgSpoolUsage: ").Append(QueueMaxMsgSpoolUsage).Append("\n");
            sb.Append("  QueueMaxRedeliveryCount: ").Append(QueueMaxRedeliveryCount).Append("\n");
            sb.Append("  QueueMaxTtl: ").Append(QueueMaxTtl).Append("\n");
            sb.Append("  QueueRejectLowPriorityMsgEnabled: ").Append(QueueRejectLowPriorityMsgEnabled).Append("\n");
            sb.Append("  QueueRejectLowPriorityMsgLimit: ").Append(QueueRejectLowPriorityMsgLimit).Append("\n");
            sb.Append("  QueueRejectMsgToSenderOnDiscardBehavior: ").Append(QueueRejectMsgToSenderOnDiscardBehavior).Append("\n");
            sb.Append("  QueueRespectTtlEnabled: ").Append(QueueRespectTtlEnabled).Append("\n");
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
            return this.Equals(input as MsgVpnMqttSession);
        }

        /// <summary>
        /// Returns true if MsgVpnMqttSession instances are equal
        /// </summary>
        /// <param name="input">Instance of MsgVpnMqttSession to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(MsgVpnMqttSession input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Enabled == input.Enabled ||
                    (this.Enabled != null &&
                    this.Enabled.Equals(input.Enabled))
                ) && 
                (
                    this.MqttSessionClientId == input.MqttSessionClientId ||
                    (this.MqttSessionClientId != null &&
                    this.MqttSessionClientId.Equals(input.MqttSessionClientId))
                ) && 
                (
                    this.MqttSessionVirtualRouter == input.MqttSessionVirtualRouter ||
                    (this.MqttSessionVirtualRouter != null &&
                    this.MqttSessionVirtualRouter.Equals(input.MqttSessionVirtualRouter))
                ) && 
                (
                    this.MsgVpnName == input.MsgVpnName ||
                    (this.MsgVpnName != null &&
                    this.MsgVpnName.Equals(input.MsgVpnName))
                ) && 
                (
                    this.Owner == input.Owner ||
                    (this.Owner != null &&
                    this.Owner.Equals(input.Owner))
                ) && 
                (
                    this.QueueConsumerAckPropagationEnabled == input.QueueConsumerAckPropagationEnabled ||
                    (this.QueueConsumerAckPropagationEnabled != null &&
                    this.QueueConsumerAckPropagationEnabled.Equals(input.QueueConsumerAckPropagationEnabled))
                ) && 
                (
                    this.QueueDeadMsgQueue == input.QueueDeadMsgQueue ||
                    (this.QueueDeadMsgQueue != null &&
                    this.QueueDeadMsgQueue.Equals(input.QueueDeadMsgQueue))
                ) && 
                (
                    this.QueueEventBindCountThreshold == input.QueueEventBindCountThreshold ||
                    (this.QueueEventBindCountThreshold != null &&
                    this.QueueEventBindCountThreshold.Equals(input.QueueEventBindCountThreshold))
                ) && 
                (
                    this.QueueEventMsgSpoolUsageThreshold == input.QueueEventMsgSpoolUsageThreshold ||
                    (this.QueueEventMsgSpoolUsageThreshold != null &&
                    this.QueueEventMsgSpoolUsageThreshold.Equals(input.QueueEventMsgSpoolUsageThreshold))
                ) && 
                (
                    this.QueueEventRejectLowPriorityMsgLimitThreshold == input.QueueEventRejectLowPriorityMsgLimitThreshold ||
                    (this.QueueEventRejectLowPriorityMsgLimitThreshold != null &&
                    this.QueueEventRejectLowPriorityMsgLimitThreshold.Equals(input.QueueEventRejectLowPriorityMsgLimitThreshold))
                ) && 
                (
                    this.QueueMaxBindCount == input.QueueMaxBindCount ||
                    (this.QueueMaxBindCount != null &&
                    this.QueueMaxBindCount.Equals(input.QueueMaxBindCount))
                ) && 
                (
                    this.QueueMaxDeliveredUnackedMsgsPerFlow == input.QueueMaxDeliveredUnackedMsgsPerFlow ||
                    (this.QueueMaxDeliveredUnackedMsgsPerFlow != null &&
                    this.QueueMaxDeliveredUnackedMsgsPerFlow.Equals(input.QueueMaxDeliveredUnackedMsgsPerFlow))
                ) && 
                (
                    this.QueueMaxMsgSize == input.QueueMaxMsgSize ||
                    (this.QueueMaxMsgSize != null &&
                    this.QueueMaxMsgSize.Equals(input.QueueMaxMsgSize))
                ) && 
                (
                    this.QueueMaxMsgSpoolUsage == input.QueueMaxMsgSpoolUsage ||
                    (this.QueueMaxMsgSpoolUsage != null &&
                    this.QueueMaxMsgSpoolUsage.Equals(input.QueueMaxMsgSpoolUsage))
                ) && 
                (
                    this.QueueMaxRedeliveryCount == input.QueueMaxRedeliveryCount ||
                    (this.QueueMaxRedeliveryCount != null &&
                    this.QueueMaxRedeliveryCount.Equals(input.QueueMaxRedeliveryCount))
                ) && 
                (
                    this.QueueMaxTtl == input.QueueMaxTtl ||
                    (this.QueueMaxTtl != null &&
                    this.QueueMaxTtl.Equals(input.QueueMaxTtl))
                ) && 
                (
                    this.QueueRejectLowPriorityMsgEnabled == input.QueueRejectLowPriorityMsgEnabled ||
                    (this.QueueRejectLowPriorityMsgEnabled != null &&
                    this.QueueRejectLowPriorityMsgEnabled.Equals(input.QueueRejectLowPriorityMsgEnabled))
                ) && 
                (
                    this.QueueRejectLowPriorityMsgLimit == input.QueueRejectLowPriorityMsgLimit ||
                    (this.QueueRejectLowPriorityMsgLimit != null &&
                    this.QueueRejectLowPriorityMsgLimit.Equals(input.QueueRejectLowPriorityMsgLimit))
                ) && 
                (
                    this.QueueRejectMsgToSenderOnDiscardBehavior == input.QueueRejectMsgToSenderOnDiscardBehavior ||
                    (this.QueueRejectMsgToSenderOnDiscardBehavior != null &&
                    this.QueueRejectMsgToSenderOnDiscardBehavior.Equals(input.QueueRejectMsgToSenderOnDiscardBehavior))
                ) && 
                (
                    this.QueueRespectTtlEnabled == input.QueueRespectTtlEnabled ||
                    (this.QueueRespectTtlEnabled != null &&
                    this.QueueRespectTtlEnabled.Equals(input.QueueRespectTtlEnabled))
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
                if (this.Enabled != null)
                    hashCode = hashCode * 59 + this.Enabled.GetHashCode();
                if (this.MqttSessionClientId != null)
                    hashCode = hashCode * 59 + this.MqttSessionClientId.GetHashCode();
                if (this.MqttSessionVirtualRouter != null)
                    hashCode = hashCode * 59 + this.MqttSessionVirtualRouter.GetHashCode();
                if (this.MsgVpnName != null)
                    hashCode = hashCode * 59 + this.MsgVpnName.GetHashCode();
                if (this.Owner != null)
                    hashCode = hashCode * 59 + this.Owner.GetHashCode();
                if (this.QueueConsumerAckPropagationEnabled != null)
                    hashCode = hashCode * 59 + this.QueueConsumerAckPropagationEnabled.GetHashCode();
                if (this.QueueDeadMsgQueue != null)
                    hashCode = hashCode * 59 + this.QueueDeadMsgQueue.GetHashCode();
                if (this.QueueEventBindCountThreshold != null)
                    hashCode = hashCode * 59 + this.QueueEventBindCountThreshold.GetHashCode();
                if (this.QueueEventMsgSpoolUsageThreshold != null)
                    hashCode = hashCode * 59 + this.QueueEventMsgSpoolUsageThreshold.GetHashCode();
                if (this.QueueEventRejectLowPriorityMsgLimitThreshold != null)
                    hashCode = hashCode * 59 + this.QueueEventRejectLowPriorityMsgLimitThreshold.GetHashCode();
                if (this.QueueMaxBindCount != null)
                    hashCode = hashCode * 59 + this.QueueMaxBindCount.GetHashCode();
                if (this.QueueMaxDeliveredUnackedMsgsPerFlow != null)
                    hashCode = hashCode * 59 + this.QueueMaxDeliveredUnackedMsgsPerFlow.GetHashCode();
                if (this.QueueMaxMsgSize != null)
                    hashCode = hashCode * 59 + this.QueueMaxMsgSize.GetHashCode();
                if (this.QueueMaxMsgSpoolUsage != null)
                    hashCode = hashCode * 59 + this.QueueMaxMsgSpoolUsage.GetHashCode();
                if (this.QueueMaxRedeliveryCount != null)
                    hashCode = hashCode * 59 + this.QueueMaxRedeliveryCount.GetHashCode();
                if (this.QueueMaxTtl != null)
                    hashCode = hashCode * 59 + this.QueueMaxTtl.GetHashCode();
                if (this.QueueRejectLowPriorityMsgEnabled != null)
                    hashCode = hashCode * 59 + this.QueueRejectLowPriorityMsgEnabled.GetHashCode();
                if (this.QueueRejectLowPriorityMsgLimit != null)
                    hashCode = hashCode * 59 + this.QueueRejectLowPriorityMsgLimit.GetHashCode();
                if (this.QueueRejectMsgToSenderOnDiscardBehavior != null)
                    hashCode = hashCode * 59 + this.QueueRejectMsgToSenderOnDiscardBehavior.GetHashCode();
                if (this.QueueRespectTtlEnabled != null)
                    hashCode = hashCode * 59 + this.QueueRespectTtlEnabled.GetHashCode();
                return hashCode;
            }
        }
    }
}
