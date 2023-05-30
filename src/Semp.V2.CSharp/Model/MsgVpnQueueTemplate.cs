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
    /// MsgVpnQueueTemplate
    /// </summary>
    [DataContract]
        public partial class MsgVpnQueueTemplate :  IEquatable<MsgVpnQueueTemplate>
    {
        /// <summary>
        /// The access type for delivering messages to consumer flows. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;exclusive\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;exclusive\&quot; - Exclusive delivery of messages to the first bound consumer flow. \&quot;non-exclusive\&quot; - Non-exclusive delivery of messages to bound consumer flows in a round-robin (if partition count is zero) or partitioned (if partition count is non-zero) fashion. &lt;/pre&gt; 
        /// </summary>
        /// <value>The access type for delivering messages to consumer flows. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;exclusive\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;exclusive\&quot; - Exclusive delivery of messages to the first bound consumer flow. \&quot;non-exclusive\&quot; - Non-exclusive delivery of messages to bound consumer flows in a round-robin (if partition count is zero) or partitioned (if partition count is non-zero) fashion. &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum AccessTypeEnum
        {
            /// <summary>
            /// Enum Exclusive for value: exclusive
            /// </summary>
            [EnumMember(Value = "exclusive")]
            Exclusive = 1,
            /// <summary>
            /// Enum NonExclusive for value: non-exclusive
            /// </summary>
            [EnumMember(Value = "non-exclusive")]
            NonExclusive = 2        }
        /// <summary>
        /// The access type for delivering messages to consumer flows. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;exclusive\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;exclusive\&quot; - Exclusive delivery of messages to the first bound consumer flow. \&quot;non-exclusive\&quot; - Non-exclusive delivery of messages to bound consumer flows in a round-robin (if partition count is zero) or partitioned (if partition count is non-zero) fashion. &lt;/pre&gt; 
        /// </summary>
        /// <value>The access type for delivering messages to consumer flows. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;exclusive\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;exclusive\&quot; - Exclusive delivery of messages to the first bound consumer flow. \&quot;non-exclusive\&quot; - Non-exclusive delivery of messages to bound consumer flows in a round-robin (if partition count is zero) or partitioned (if partition count is non-zero) fashion. &lt;/pre&gt; </value>
        [DataMember(Name="accessType", EmitDefaultValue=false)]
        public AccessTypeEnum? AccessType { get; set; }
        /// <summary>
        /// Controls the durability of queues created from this template. If non-durable, the created queue will be non-durable, regardless of the specified durability. If none, the created queue will have the requested durability. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - The durability of the endpoint will be as requested on create. \&quot;non-durable\&quot; - The durability of the created queue will be non-durable, regardless of what was requested. &lt;/pre&gt; 
        /// </summary>
        /// <value>Controls the durability of queues created from this template. If non-durable, the created queue will be non-durable, regardless of the specified durability. If none, the created queue will have the requested durability. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - The durability of the endpoint will be as requested on create. \&quot;non-durable\&quot; - The durability of the created queue will be non-durable, regardless of what was requested. &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum DurabilityOverrideEnum
        {
            /// <summary>
            /// Enum None for value: none
            /// </summary>
            [EnumMember(Value = "none")]
            None = 1,
            /// <summary>
            /// Enum NonDurable for value: non-durable
            /// </summary>
            [EnumMember(Value = "non-durable")]
            NonDurable = 2        }
        /// <summary>
        /// Controls the durability of queues created from this template. If non-durable, the created queue will be non-durable, regardless of the specified durability. If none, the created queue will have the requested durability. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - The durability of the endpoint will be as requested on create. \&quot;non-durable\&quot; - The durability of the created queue will be non-durable, regardless of what was requested. &lt;/pre&gt; 
        /// </summary>
        /// <value>Controls the durability of queues created from this template. If non-durable, the created queue will be non-durable, regardless of the specified durability. If none, the created queue will have the requested durability. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - The durability of the endpoint will be as requested on create. \&quot;non-durable\&quot; - The durability of the created queue will be non-durable, regardless of what was requested. &lt;/pre&gt; </value>
        [DataMember(Name="durabilityOverride", EmitDefaultValue=false)]
        public DurabilityOverrideEnum? DurabilityOverride { get; set; }
        /// <summary>
        /// The permission level for all consumers, excluding the owner. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;no-access\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;no-access\&quot; - Disallows all access. \&quot;read-only\&quot; - Read-only access to the messages. \&quot;consume\&quot; - Consume (read and remove) messages. \&quot;modify-topic\&quot; - Consume messages or modify the topic/selector. \&quot;delete\&quot; - Consume messages, modify the topic/selector or delete the Client created endpoint altogether. &lt;/pre&gt; 
        /// </summary>
        /// <value>The permission level for all consumers, excluding the owner. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;no-access\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;no-access\&quot; - Disallows all access. \&quot;read-only\&quot; - Read-only access to the messages. \&quot;consume\&quot; - Consume (read and remove) messages. \&quot;modify-topic\&quot; - Consume messages or modify the topic/selector. \&quot;delete\&quot; - Consume messages, modify the topic/selector or delete the Client created endpoint altogether. &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum PermissionEnum
        {
            /// <summary>
            /// Enum NoAccess for value: no-access
            /// </summary>
            [EnumMember(Value = "no-access")]
            NoAccess = 1,
            /// <summary>
            /// Enum ReadOnly for value: read-only
            /// </summary>
            [EnumMember(Value = "read-only")]
            ReadOnly = 2,
            /// <summary>
            /// Enum Consume for value: consume
            /// </summary>
            [EnumMember(Value = "consume")]
            Consume = 3,
            /// <summary>
            /// Enum ModifyTopic for value: modify-topic
            /// </summary>
            [EnumMember(Value = "modify-topic")]
            ModifyTopic = 4,
            /// <summary>
            /// Enum Delete for value: delete
            /// </summary>
            [EnumMember(Value = "delete")]
            Delete = 5        }
        /// <summary>
        /// The permission level for all consumers, excluding the owner. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;no-access\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;no-access\&quot; - Disallows all access. \&quot;read-only\&quot; - Read-only access to the messages. \&quot;consume\&quot; - Consume (read and remove) messages. \&quot;modify-topic\&quot; - Consume messages or modify the topic/selector. \&quot;delete\&quot; - Consume messages, modify the topic/selector or delete the Client created endpoint altogether. &lt;/pre&gt; 
        /// </summary>
        /// <value>The permission level for all consumers, excluding the owner. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;no-access\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;no-access\&quot; - Disallows all access. \&quot;read-only\&quot; - Read-only access to the messages. \&quot;consume\&quot; - Consume (read and remove) messages. \&quot;modify-topic\&quot; - Consume messages or modify the topic/selector. \&quot;delete\&quot; - Consume messages, modify the topic/selector or delete the Client created endpoint altogether. &lt;/pre&gt; </value>
        [DataMember(Name="permission", EmitDefaultValue=false)]
        public PermissionEnum? Permission { get; set; }
        /// <summary>
        /// Determines when to return negative acknowledgements (NACKs) to sending clients on message discards. Note that NACKs prevent the message from being delivered to any destination and Transacted Session commits to fail. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as rejectLowPriorityMsgEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-queue-enabled\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always return a negative acknowledgment (NACK) to the sending client on message discard. \&quot;when-queue-enabled\&quot; - Only return a negative acknowledgment (NACK) to the sending client on message discard when the Queue is enabled. \&quot;never\&quot; - Never return a negative acknowledgment (NACK) to the sending client on message discard. &lt;/pre&gt; 
        /// </summary>
        /// <value>Determines when to return negative acknowledgements (NACKs) to sending clients on message discards. Note that NACKs prevent the message from being delivered to any destination and Transacted Session commits to fail. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as rejectLowPriorityMsgEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-queue-enabled\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always return a negative acknowledgment (NACK) to the sending client on message discard. \&quot;when-queue-enabled\&quot; - Only return a negative acknowledgment (NACK) to the sending client on message discard when the Queue is enabled. \&quot;never\&quot; - Never return a negative acknowledgment (NACK) to the sending client on message discard. &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum RejectMsgToSenderOnDiscardBehaviorEnum
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
        /// Determines when to return negative acknowledgements (NACKs) to sending clients on message discards. Note that NACKs prevent the message from being delivered to any destination and Transacted Session commits to fail. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as rejectLowPriorityMsgEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-queue-enabled\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always return a negative acknowledgment (NACK) to the sending client on message discard. \&quot;when-queue-enabled\&quot; - Only return a negative acknowledgment (NACK) to the sending client on message discard when the Queue is enabled. \&quot;never\&quot; - Never return a negative acknowledgment (NACK) to the sending client on message discard. &lt;/pre&gt; 
        /// </summary>
        /// <value>Determines when to return negative acknowledgements (NACKs) to sending clients on message discards. Note that NACKs prevent the message from being delivered to any destination and Transacted Session commits to fail. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as rejectLowPriorityMsgEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-queue-enabled\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always return a negative acknowledgment (NACK) to the sending client on message discard. \&quot;when-queue-enabled\&quot; - Only return a negative acknowledgment (NACK) to the sending client on message discard when the Queue is enabled. \&quot;never\&quot; - Never return a negative acknowledgment (NACK) to the sending client on message discard. &lt;/pre&gt; </value>
        [DataMember(Name="rejectMsgToSenderOnDiscardBehavior", EmitDefaultValue=false)]
        public RejectMsgToSenderOnDiscardBehaviorEnum? RejectMsgToSenderOnDiscardBehavior { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="MsgVpnQueueTemplate" /> class.
        /// </summary>
        /// <param name="accessType">The access type for delivering messages to consumer flows. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;exclusive\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;exclusive\&quot; - Exclusive delivery of messages to the first bound consumer flow. \&quot;non-exclusive\&quot; - Non-exclusive delivery of messages to bound consumer flows in a round-robin (if partition count is zero) or partitioned (if partition count is non-zero) fashion. &lt;/pre&gt; .</param>
        /// <param name="consumerAckPropagationEnabled">Enable or disable the propagation of consumer acknowledgements (ACKs) received on the active replication Message VPN to the standby replication Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="deadMsgQueue">The name of the Dead Message Queue (DMQ). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;#DEAD_MSG_QUEUE\&quot;&#x60;..</param>
        /// <param name="deliveryDelay">The delay, in seconds, to apply to messages arriving on the Queue before the messages are eligible for delivery. This attribute does not apply to MQTT queues created from this template, but it may apply in future releases. Therefore, to maintain forward compatibility, do not set this value on templates that might be used for MQTT queues. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.22..</param>
        /// <param name="durabilityOverride">Controls the durability of queues created from this template. If non-durable, the created queue will be non-durable, regardless of the specified durability. If none, the created queue will have the requested durability. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - The durability of the endpoint will be as requested on create. \&quot;non-durable\&quot; - The durability of the created queue will be non-durable, regardless of what was requested. &lt;/pre&gt; .</param>
        /// <param name="eventBindCountThreshold">eventBindCountThreshold.</param>
        /// <param name="eventMsgSpoolUsageThreshold">eventMsgSpoolUsageThreshold.</param>
        /// <param name="eventRejectLowPriorityMsgLimitThreshold">eventRejectLowPriorityMsgLimitThreshold.</param>
        /// <param name="maxBindCount">The maximum number of consumer flows that can bind. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;..</param>
        /// <param name="maxDeliveredUnackedMsgsPerFlow">The maximum number of messages delivered but not acknowledged per flow. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;10000&#x60;..</param>
        /// <param name="maxMsgSize">The maximum message size allowed, in bytes (B). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;10000000&#x60;..</param>
        /// <param name="maxMsgSpoolUsage">The maximum message spool usage allowed, in megabytes (MB). A value of 0 only allows spooling of the last message received and disables quota checking. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;5000&#x60;..</param>
        /// <param name="maxRedeliveryCount">The maximum number of message redelivery attempts that will occur prior to the message being discarded or moved to the DMQ. A value of 0 means to retry forever. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;..</param>
        /// <param name="maxTtl">The maximum time in seconds a message can stay in a Queue when &#x60;respectTtlEnabled&#x60; is &#x60;\&quot;true\&quot;&#x60;. A message expires when the lesser of the sender assigned time-to-live (TTL) in the message and the &#x60;maxTtl&#x60; configured for the Queue, is exceeded. A value of 0 disables expiry. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;..</param>
        /// <param name="msgVpnName">The name of the Message VPN..</param>
        /// <param name="permission">The permission level for all consumers, excluding the owner. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;no-access\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;no-access\&quot; - Disallows all access. \&quot;read-only\&quot; - Read-only access to the messages. \&quot;consume\&quot; - Consume (read and remove) messages. \&quot;modify-topic\&quot; - Consume messages or modify the topic/selector. \&quot;delete\&quot; - Consume messages, modify the topic/selector or delete the Client created endpoint altogether. &lt;/pre&gt; .</param>
        /// <param name="queueNameFilter">A wildcardable pattern used to determine which Queues use settings from this Template. Two different wildcards are supported: * and &gt;. Similar to topic filters or subscription patterns, a &gt; matches anything (but only when used at the end), and a * matches zero or more characters but never a slash (/). A &gt; is only a wildcard when used at the end, after a /. A * is only allowed at the end, after a slash (/). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="queueTemplateName">The name of the Queue Template..</param>
        /// <param name="redeliveryDelayEnabled">Enable or disable a message redelivery delay. When false, messages are redelivered as soon as possible.  When true, messages are redelivered according to the initial, max and multiplier.  This should only be enabled when redelivery is enabled. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.33..</param>
        /// <param name="redeliveryDelayInitialInterval">The delay to be used between the first 2 redelivery attempts.  This value is in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;. Available since 2.33..</param>
        /// <param name="redeliveryDelayMaxInterval">The maximum delay to be used between any 2 redelivery attempts.  This value is in milliseconds.  Due to technical limitations, some redelivery attempt delays may slightly exceed this value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;64000&#x60;. Available since 2.33..</param>
        /// <param name="redeliveryDelayMultiplier">The amount each delay interval is multiplied by after each failed delivery attempt.  This number is in a fixed-point decimal format in which you must divide by 100 to get the floating point value. For example, a value of 125 would cause the delay to be multiplied by 1.25. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;200&#x60;. Available since 2.33..</param>
        /// <param name="redeliveryEnabled">Enable or disable message redelivery. When enabled, the number of redelivery attempts is controlled by maxRedeliveryCount. When disabled, the message will never be delivered from the queue more than once. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;. Available since 2.18..</param>
        /// <param name="rejectLowPriorityMsgEnabled">Enable or disable the checking of low priority messages against the &#x60;rejectLowPriorityMsgLimit&#x60;. This may only be enabled if &#x60;rejectMsgToSenderOnDiscardBehavior&#x60; does not have a value of &#x60;\&quot;never\&quot;&#x60;. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="rejectLowPriorityMsgLimit">The number of messages of any priority above which low priority messages are not admitted but higher priority messages are allowed. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;..</param>
        /// <param name="rejectMsgToSenderOnDiscardBehavior">Determines when to return negative acknowledgements (NACKs) to sending clients on message discards. Note that NACKs prevent the message from being delivered to any destination and Transacted Session commits to fail. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as rejectLowPriorityMsgEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-queue-enabled\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always return a negative acknowledgment (NACK) to the sending client on message discard. \&quot;when-queue-enabled\&quot; - Only return a negative acknowledgment (NACK) to the sending client on message discard when the Queue is enabled. \&quot;never\&quot; - Never return a negative acknowledgment (NACK) to the sending client on message discard. &lt;/pre&gt; .</param>
        /// <param name="respectMsgPriorityEnabled">Enable or disable the respecting of message priority. When enabled, messages are delivered in priority order, from 9 (highest) to 0 (lowest). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="respectTtlEnabled">Enable or disable the respecting of the time-to-live (TTL) for messages. When enabled, expired messages are discarded or moved to the DMQ. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        public MsgVpnQueueTemplate(AccessTypeEnum? accessType = default(AccessTypeEnum?), bool? consumerAckPropagationEnabled = default(bool?), string deadMsgQueue = default(string), long? deliveryDelay = default(long?), DurabilityOverrideEnum? durabilityOverride = default(DurabilityOverrideEnum?), EventThreshold eventBindCountThreshold = default(EventThreshold), EventThreshold eventMsgSpoolUsageThreshold = default(EventThreshold), EventThreshold eventRejectLowPriorityMsgLimitThreshold = default(EventThreshold), long? maxBindCount = default(long?), long? maxDeliveredUnackedMsgsPerFlow = default(long?), int? maxMsgSize = default(int?), long? maxMsgSpoolUsage = default(long?), long? maxRedeliveryCount = default(long?), long? maxTtl = default(long?), string msgVpnName = default(string), PermissionEnum? permission = default(PermissionEnum?), string queueNameFilter = default(string), string queueTemplateName = default(string), bool? redeliveryDelayEnabled = default(bool?), int? redeliveryDelayInitialInterval = default(int?), int? redeliveryDelayMaxInterval = default(int?), int? redeliveryDelayMultiplier = default(int?), bool? redeliveryEnabled = default(bool?), bool? rejectLowPriorityMsgEnabled = default(bool?), long? rejectLowPriorityMsgLimit = default(long?), RejectMsgToSenderOnDiscardBehaviorEnum? rejectMsgToSenderOnDiscardBehavior = default(RejectMsgToSenderOnDiscardBehaviorEnum?), bool? respectMsgPriorityEnabled = default(bool?), bool? respectTtlEnabled = default(bool?))
        {
            this.AccessType = accessType;
            this.ConsumerAckPropagationEnabled = consumerAckPropagationEnabled;
            this.DeadMsgQueue = deadMsgQueue;
            this.DeliveryDelay = deliveryDelay;
            this.DurabilityOverride = durabilityOverride;
            this.EventBindCountThreshold = eventBindCountThreshold;
            this.EventMsgSpoolUsageThreshold = eventMsgSpoolUsageThreshold;
            this.EventRejectLowPriorityMsgLimitThreshold = eventRejectLowPriorityMsgLimitThreshold;
            this.MaxBindCount = maxBindCount;
            this.MaxDeliveredUnackedMsgsPerFlow = maxDeliveredUnackedMsgsPerFlow;
            this.MaxMsgSize = maxMsgSize;
            this.MaxMsgSpoolUsage = maxMsgSpoolUsage;
            this.MaxRedeliveryCount = maxRedeliveryCount;
            this.MaxTtl = maxTtl;
            this.MsgVpnName = msgVpnName;
            this.Permission = permission;
            this.QueueNameFilter = queueNameFilter;
            this.QueueTemplateName = queueTemplateName;
            this.RedeliveryDelayEnabled = redeliveryDelayEnabled;
            this.RedeliveryDelayInitialInterval = redeliveryDelayInitialInterval;
            this.RedeliveryDelayMaxInterval = redeliveryDelayMaxInterval;
            this.RedeliveryDelayMultiplier = redeliveryDelayMultiplier;
            this.RedeliveryEnabled = redeliveryEnabled;
            this.RejectLowPriorityMsgEnabled = rejectLowPriorityMsgEnabled;
            this.RejectLowPriorityMsgLimit = rejectLowPriorityMsgLimit;
            this.RejectMsgToSenderOnDiscardBehavior = rejectMsgToSenderOnDiscardBehavior;
            this.RespectMsgPriorityEnabled = respectMsgPriorityEnabled;
            this.RespectTtlEnabled = respectTtlEnabled;
        }
        

        /// <summary>
        /// Enable or disable the propagation of consumer acknowledgements (ACKs) received on the active replication Message VPN to the standby replication Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable the propagation of consumer acknowledgements (ACKs) received on the active replication Message VPN to the standby replication Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="consumerAckPropagationEnabled", EmitDefaultValue=false)]
        public bool? ConsumerAckPropagationEnabled { get; set; }

        /// <summary>
        /// The name of the Dead Message Queue (DMQ). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;#DEAD_MSG_QUEUE\&quot;&#x60;.
        /// </summary>
        /// <value>The name of the Dead Message Queue (DMQ). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;#DEAD_MSG_QUEUE\&quot;&#x60;.</value>
        [DataMember(Name="deadMsgQueue", EmitDefaultValue=false)]
        public string DeadMsgQueue { get; set; }

        /// <summary>
        /// The delay, in seconds, to apply to messages arriving on the Queue before the messages are eligible for delivery. This attribute does not apply to MQTT queues created from this template, but it may apply in future releases. Therefore, to maintain forward compatibility, do not set this value on templates that might be used for MQTT queues. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.22.
        /// </summary>
        /// <value>The delay, in seconds, to apply to messages arriving on the Queue before the messages are eligible for delivery. This attribute does not apply to MQTT queues created from this template, but it may apply in future releases. Therefore, to maintain forward compatibility, do not set this value on templates that might be used for MQTT queues. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.22.</value>
        [DataMember(Name="deliveryDelay", EmitDefaultValue=false)]
        public long? DeliveryDelay { get; set; }


        /// <summary>
        /// Gets or Sets EventBindCountThreshold
        /// </summary>
        [DataMember(Name="eventBindCountThreshold", EmitDefaultValue=false)]
        public EventThreshold EventBindCountThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventMsgSpoolUsageThreshold
        /// </summary>
        [DataMember(Name="eventMsgSpoolUsageThreshold", EmitDefaultValue=false)]
        public EventThreshold EventMsgSpoolUsageThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventRejectLowPriorityMsgLimitThreshold
        /// </summary>
        [DataMember(Name="eventRejectLowPriorityMsgLimitThreshold", EmitDefaultValue=false)]
        public EventThreshold EventRejectLowPriorityMsgLimitThreshold { get; set; }

        /// <summary>
        /// The maximum number of consumer flows that can bind. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;.
        /// </summary>
        /// <value>The maximum number of consumer flows that can bind. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;.</value>
        [DataMember(Name="maxBindCount", EmitDefaultValue=false)]
        public long? MaxBindCount { get; set; }

        /// <summary>
        /// The maximum number of messages delivered but not acknowledged per flow. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;10000&#x60;.
        /// </summary>
        /// <value>The maximum number of messages delivered but not acknowledged per flow. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;10000&#x60;.</value>
        [DataMember(Name="maxDeliveredUnackedMsgsPerFlow", EmitDefaultValue=false)]
        public long? MaxDeliveredUnackedMsgsPerFlow { get; set; }

        /// <summary>
        /// The maximum message size allowed, in bytes (B). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;10000000&#x60;.
        /// </summary>
        /// <value>The maximum message size allowed, in bytes (B). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;10000000&#x60;.</value>
        [DataMember(Name="maxMsgSize", EmitDefaultValue=false)]
        public int? MaxMsgSize { get; set; }

        /// <summary>
        /// The maximum message spool usage allowed, in megabytes (MB). A value of 0 only allows spooling of the last message received and disables quota checking. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;5000&#x60;.
        /// </summary>
        /// <value>The maximum message spool usage allowed, in megabytes (MB). A value of 0 only allows spooling of the last message received and disables quota checking. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;5000&#x60;.</value>
        [DataMember(Name="maxMsgSpoolUsage", EmitDefaultValue=false)]
        public long? MaxMsgSpoolUsage { get; set; }

        /// <summary>
        /// The maximum number of message redelivery attempts that will occur prior to the message being discarded or moved to the DMQ. A value of 0 means to retry forever. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;.
        /// </summary>
        /// <value>The maximum number of message redelivery attempts that will occur prior to the message being discarded or moved to the DMQ. A value of 0 means to retry forever. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;.</value>
        [DataMember(Name="maxRedeliveryCount", EmitDefaultValue=false)]
        public long? MaxRedeliveryCount { get; set; }

        /// <summary>
        /// The maximum time in seconds a message can stay in a Queue when &#x60;respectTtlEnabled&#x60; is &#x60;\&quot;true\&quot;&#x60;. A message expires when the lesser of the sender assigned time-to-live (TTL) in the message and the &#x60;maxTtl&#x60; configured for the Queue, is exceeded. A value of 0 disables expiry. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;.
        /// </summary>
        /// <value>The maximum time in seconds a message can stay in a Queue when &#x60;respectTtlEnabled&#x60; is &#x60;\&quot;true\&quot;&#x60;. A message expires when the lesser of the sender assigned time-to-live (TTL) in the message and the &#x60;maxTtl&#x60; configured for the Queue, is exceeded. A value of 0 disables expiry. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;.</value>
        [DataMember(Name="maxTtl", EmitDefaultValue=false)]
        public long? MaxTtl { get; set; }

        /// <summary>
        /// The name of the Message VPN.
        /// </summary>
        /// <value>The name of the Message VPN.</value>
        [DataMember(Name="msgVpnName", EmitDefaultValue=false)]
        public string MsgVpnName { get; set; }


        /// <summary>
        /// A wildcardable pattern used to determine which Queues use settings from this Template. Two different wildcards are supported: * and &gt;. Similar to topic filters or subscription patterns, a &gt; matches anything (but only when used at the end), and a * matches zero or more characters but never a slash (/). A &gt; is only a wildcard when used at the end, after a /. A * is only allowed at the end, after a slash (/). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>A wildcardable pattern used to determine which Queues use settings from this Template. Two different wildcards are supported: * and &gt;. Similar to topic filters or subscription patterns, a &gt; matches anything (but only when used at the end), and a * matches zero or more characters but never a slash (/). A &gt; is only a wildcard when used at the end, after a /. A * is only allowed at the end, after a slash (/). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="queueNameFilter", EmitDefaultValue=false)]
        public string QueueNameFilter { get; set; }

        /// <summary>
        /// The name of the Queue Template.
        /// </summary>
        /// <value>The name of the Queue Template.</value>
        [DataMember(Name="queueTemplateName", EmitDefaultValue=false)]
        public string QueueTemplateName { get; set; }

        /// <summary>
        /// Enable or disable a message redelivery delay. When false, messages are redelivered as soon as possible.  When true, messages are redelivered according to the initial, max and multiplier.  This should only be enabled when redelivery is enabled. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.33.
        /// </summary>
        /// <value>Enable or disable a message redelivery delay. When false, messages are redelivered as soon as possible.  When true, messages are redelivered according to the initial, max and multiplier.  This should only be enabled when redelivery is enabled. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.33.</value>
        [DataMember(Name="redeliveryDelayEnabled", EmitDefaultValue=false)]
        public bool? RedeliveryDelayEnabled { get; set; }

        /// <summary>
        /// The delay to be used between the first 2 redelivery attempts.  This value is in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;. Available since 2.33.
        /// </summary>
        /// <value>The delay to be used between the first 2 redelivery attempts.  This value is in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;. Available since 2.33.</value>
        [DataMember(Name="redeliveryDelayInitialInterval", EmitDefaultValue=false)]
        public int? RedeliveryDelayInitialInterval { get; set; }

        /// <summary>
        /// The maximum delay to be used between any 2 redelivery attempts.  This value is in milliseconds.  Due to technical limitations, some redelivery attempt delays may slightly exceed this value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;64000&#x60;. Available since 2.33.
        /// </summary>
        /// <value>The maximum delay to be used between any 2 redelivery attempts.  This value is in milliseconds.  Due to technical limitations, some redelivery attempt delays may slightly exceed this value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;64000&#x60;. Available since 2.33.</value>
        [DataMember(Name="redeliveryDelayMaxInterval", EmitDefaultValue=false)]
        public int? RedeliveryDelayMaxInterval { get; set; }

        /// <summary>
        /// The amount each delay interval is multiplied by after each failed delivery attempt.  This number is in a fixed-point decimal format in which you must divide by 100 to get the floating point value. For example, a value of 125 would cause the delay to be multiplied by 1.25. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;200&#x60;. Available since 2.33.
        /// </summary>
        /// <value>The amount each delay interval is multiplied by after each failed delivery attempt.  This number is in a fixed-point decimal format in which you must divide by 100 to get the floating point value. For example, a value of 125 would cause the delay to be multiplied by 1.25. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;200&#x60;. Available since 2.33.</value>
        [DataMember(Name="redeliveryDelayMultiplier", EmitDefaultValue=false)]
        public int? RedeliveryDelayMultiplier { get; set; }

        /// <summary>
        /// Enable or disable message redelivery. When enabled, the number of redelivery attempts is controlled by maxRedeliveryCount. When disabled, the message will never be delivered from the queue more than once. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;. Available since 2.18.
        /// </summary>
        /// <value>Enable or disable message redelivery. When enabled, the number of redelivery attempts is controlled by maxRedeliveryCount. When disabled, the message will never be delivered from the queue more than once. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;. Available since 2.18.</value>
        [DataMember(Name="redeliveryEnabled", EmitDefaultValue=false)]
        public bool? RedeliveryEnabled { get; set; }

        /// <summary>
        /// Enable or disable the checking of low priority messages against the &#x60;rejectLowPriorityMsgLimit&#x60;. This may only be enabled if &#x60;rejectMsgToSenderOnDiscardBehavior&#x60; does not have a value of &#x60;\&quot;never\&quot;&#x60;. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable the checking of low priority messages against the &#x60;rejectLowPriorityMsgLimit&#x60;. This may only be enabled if &#x60;rejectMsgToSenderOnDiscardBehavior&#x60; does not have a value of &#x60;\&quot;never\&quot;&#x60;. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="rejectLowPriorityMsgEnabled", EmitDefaultValue=false)]
        public bool? RejectLowPriorityMsgEnabled { get; set; }

        /// <summary>
        /// The number of messages of any priority above which low priority messages are not admitted but higher priority messages are allowed. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;.
        /// </summary>
        /// <value>The number of messages of any priority above which low priority messages are not admitted but higher priority messages are allowed. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;.</value>
        [DataMember(Name="rejectLowPriorityMsgLimit", EmitDefaultValue=false)]
        public long? RejectLowPriorityMsgLimit { get; set; }


        /// <summary>
        /// Enable or disable the respecting of message priority. When enabled, messages are delivered in priority order, from 9 (highest) to 0 (lowest). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable the respecting of message priority. When enabled, messages are delivered in priority order, from 9 (highest) to 0 (lowest). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="respectMsgPriorityEnabled", EmitDefaultValue=false)]
        public bool? RespectMsgPriorityEnabled { get; set; }

        /// <summary>
        /// Enable or disable the respecting of the time-to-live (TTL) for messages. When enabled, expired messages are discarded or moved to the DMQ. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable the respecting of the time-to-live (TTL) for messages. When enabled, expired messages are discarded or moved to the DMQ. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="respectTtlEnabled", EmitDefaultValue=false)]
        public bool? RespectTtlEnabled { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class MsgVpnQueueTemplate {\n");
            sb.Append("  AccessType: ").Append(AccessType).Append("\n");
            sb.Append("  ConsumerAckPropagationEnabled: ").Append(ConsumerAckPropagationEnabled).Append("\n");
            sb.Append("  DeadMsgQueue: ").Append(DeadMsgQueue).Append("\n");
            sb.Append("  DeliveryDelay: ").Append(DeliveryDelay).Append("\n");
            sb.Append("  DurabilityOverride: ").Append(DurabilityOverride).Append("\n");
            sb.Append("  EventBindCountThreshold: ").Append(EventBindCountThreshold).Append("\n");
            sb.Append("  EventMsgSpoolUsageThreshold: ").Append(EventMsgSpoolUsageThreshold).Append("\n");
            sb.Append("  EventRejectLowPriorityMsgLimitThreshold: ").Append(EventRejectLowPriorityMsgLimitThreshold).Append("\n");
            sb.Append("  MaxBindCount: ").Append(MaxBindCount).Append("\n");
            sb.Append("  MaxDeliveredUnackedMsgsPerFlow: ").Append(MaxDeliveredUnackedMsgsPerFlow).Append("\n");
            sb.Append("  MaxMsgSize: ").Append(MaxMsgSize).Append("\n");
            sb.Append("  MaxMsgSpoolUsage: ").Append(MaxMsgSpoolUsage).Append("\n");
            sb.Append("  MaxRedeliveryCount: ").Append(MaxRedeliveryCount).Append("\n");
            sb.Append("  MaxTtl: ").Append(MaxTtl).Append("\n");
            sb.Append("  MsgVpnName: ").Append(MsgVpnName).Append("\n");
            sb.Append("  Permission: ").Append(Permission).Append("\n");
            sb.Append("  QueueNameFilter: ").Append(QueueNameFilter).Append("\n");
            sb.Append("  QueueTemplateName: ").Append(QueueTemplateName).Append("\n");
            sb.Append("  RedeliveryDelayEnabled: ").Append(RedeliveryDelayEnabled).Append("\n");
            sb.Append("  RedeliveryDelayInitialInterval: ").Append(RedeliveryDelayInitialInterval).Append("\n");
            sb.Append("  RedeliveryDelayMaxInterval: ").Append(RedeliveryDelayMaxInterval).Append("\n");
            sb.Append("  RedeliveryDelayMultiplier: ").Append(RedeliveryDelayMultiplier).Append("\n");
            sb.Append("  RedeliveryEnabled: ").Append(RedeliveryEnabled).Append("\n");
            sb.Append("  RejectLowPriorityMsgEnabled: ").Append(RejectLowPriorityMsgEnabled).Append("\n");
            sb.Append("  RejectLowPriorityMsgLimit: ").Append(RejectLowPriorityMsgLimit).Append("\n");
            sb.Append("  RejectMsgToSenderOnDiscardBehavior: ").Append(RejectMsgToSenderOnDiscardBehavior).Append("\n");
            sb.Append("  RespectMsgPriorityEnabled: ").Append(RespectMsgPriorityEnabled).Append("\n");
            sb.Append("  RespectTtlEnabled: ").Append(RespectTtlEnabled).Append("\n");
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
            return this.Equals(input as MsgVpnQueueTemplate);
        }

        /// <summary>
        /// Returns true if MsgVpnQueueTemplate instances are equal
        /// </summary>
        /// <param name="input">Instance of MsgVpnQueueTemplate to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(MsgVpnQueueTemplate input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.AccessType == input.AccessType ||
                    (this.AccessType != null &&
                    this.AccessType.Equals(input.AccessType))
                ) && 
                (
                    this.ConsumerAckPropagationEnabled == input.ConsumerAckPropagationEnabled ||
                    (this.ConsumerAckPropagationEnabled != null &&
                    this.ConsumerAckPropagationEnabled.Equals(input.ConsumerAckPropagationEnabled))
                ) && 
                (
                    this.DeadMsgQueue == input.DeadMsgQueue ||
                    (this.DeadMsgQueue != null &&
                    this.DeadMsgQueue.Equals(input.DeadMsgQueue))
                ) && 
                (
                    this.DeliveryDelay == input.DeliveryDelay ||
                    (this.DeliveryDelay != null &&
                    this.DeliveryDelay.Equals(input.DeliveryDelay))
                ) && 
                (
                    this.DurabilityOverride == input.DurabilityOverride ||
                    (this.DurabilityOverride != null &&
                    this.DurabilityOverride.Equals(input.DurabilityOverride))
                ) && 
                (
                    this.EventBindCountThreshold == input.EventBindCountThreshold ||
                    (this.EventBindCountThreshold != null &&
                    this.EventBindCountThreshold.Equals(input.EventBindCountThreshold))
                ) && 
                (
                    this.EventMsgSpoolUsageThreshold == input.EventMsgSpoolUsageThreshold ||
                    (this.EventMsgSpoolUsageThreshold != null &&
                    this.EventMsgSpoolUsageThreshold.Equals(input.EventMsgSpoolUsageThreshold))
                ) && 
                (
                    this.EventRejectLowPriorityMsgLimitThreshold == input.EventRejectLowPriorityMsgLimitThreshold ||
                    (this.EventRejectLowPriorityMsgLimitThreshold != null &&
                    this.EventRejectLowPriorityMsgLimitThreshold.Equals(input.EventRejectLowPriorityMsgLimitThreshold))
                ) && 
                (
                    this.MaxBindCount == input.MaxBindCount ||
                    (this.MaxBindCount != null &&
                    this.MaxBindCount.Equals(input.MaxBindCount))
                ) && 
                (
                    this.MaxDeliveredUnackedMsgsPerFlow == input.MaxDeliveredUnackedMsgsPerFlow ||
                    (this.MaxDeliveredUnackedMsgsPerFlow != null &&
                    this.MaxDeliveredUnackedMsgsPerFlow.Equals(input.MaxDeliveredUnackedMsgsPerFlow))
                ) && 
                (
                    this.MaxMsgSize == input.MaxMsgSize ||
                    (this.MaxMsgSize != null &&
                    this.MaxMsgSize.Equals(input.MaxMsgSize))
                ) && 
                (
                    this.MaxMsgSpoolUsage == input.MaxMsgSpoolUsage ||
                    (this.MaxMsgSpoolUsage != null &&
                    this.MaxMsgSpoolUsage.Equals(input.MaxMsgSpoolUsage))
                ) && 
                (
                    this.MaxRedeliveryCount == input.MaxRedeliveryCount ||
                    (this.MaxRedeliveryCount != null &&
                    this.MaxRedeliveryCount.Equals(input.MaxRedeliveryCount))
                ) && 
                (
                    this.MaxTtl == input.MaxTtl ||
                    (this.MaxTtl != null &&
                    this.MaxTtl.Equals(input.MaxTtl))
                ) && 
                (
                    this.MsgVpnName == input.MsgVpnName ||
                    (this.MsgVpnName != null &&
                    this.MsgVpnName.Equals(input.MsgVpnName))
                ) && 
                (
                    this.Permission == input.Permission ||
                    (this.Permission != null &&
                    this.Permission.Equals(input.Permission))
                ) && 
                (
                    this.QueueNameFilter == input.QueueNameFilter ||
                    (this.QueueNameFilter != null &&
                    this.QueueNameFilter.Equals(input.QueueNameFilter))
                ) && 
                (
                    this.QueueTemplateName == input.QueueTemplateName ||
                    (this.QueueTemplateName != null &&
                    this.QueueTemplateName.Equals(input.QueueTemplateName))
                ) && 
                (
                    this.RedeliveryDelayEnabled == input.RedeliveryDelayEnabled ||
                    (this.RedeliveryDelayEnabled != null &&
                    this.RedeliveryDelayEnabled.Equals(input.RedeliveryDelayEnabled))
                ) && 
                (
                    this.RedeliveryDelayInitialInterval == input.RedeliveryDelayInitialInterval ||
                    (this.RedeliveryDelayInitialInterval != null &&
                    this.RedeliveryDelayInitialInterval.Equals(input.RedeliveryDelayInitialInterval))
                ) && 
                (
                    this.RedeliveryDelayMaxInterval == input.RedeliveryDelayMaxInterval ||
                    (this.RedeliveryDelayMaxInterval != null &&
                    this.RedeliveryDelayMaxInterval.Equals(input.RedeliveryDelayMaxInterval))
                ) && 
                (
                    this.RedeliveryDelayMultiplier == input.RedeliveryDelayMultiplier ||
                    (this.RedeliveryDelayMultiplier != null &&
                    this.RedeliveryDelayMultiplier.Equals(input.RedeliveryDelayMultiplier))
                ) && 
                (
                    this.RedeliveryEnabled == input.RedeliveryEnabled ||
                    (this.RedeliveryEnabled != null &&
                    this.RedeliveryEnabled.Equals(input.RedeliveryEnabled))
                ) && 
                (
                    this.RejectLowPriorityMsgEnabled == input.RejectLowPriorityMsgEnabled ||
                    (this.RejectLowPriorityMsgEnabled != null &&
                    this.RejectLowPriorityMsgEnabled.Equals(input.RejectLowPriorityMsgEnabled))
                ) && 
                (
                    this.RejectLowPriorityMsgLimit == input.RejectLowPriorityMsgLimit ||
                    (this.RejectLowPriorityMsgLimit != null &&
                    this.RejectLowPriorityMsgLimit.Equals(input.RejectLowPriorityMsgLimit))
                ) && 
                (
                    this.RejectMsgToSenderOnDiscardBehavior == input.RejectMsgToSenderOnDiscardBehavior ||
                    (this.RejectMsgToSenderOnDiscardBehavior != null &&
                    this.RejectMsgToSenderOnDiscardBehavior.Equals(input.RejectMsgToSenderOnDiscardBehavior))
                ) && 
                (
                    this.RespectMsgPriorityEnabled == input.RespectMsgPriorityEnabled ||
                    (this.RespectMsgPriorityEnabled != null &&
                    this.RespectMsgPriorityEnabled.Equals(input.RespectMsgPriorityEnabled))
                ) && 
                (
                    this.RespectTtlEnabled == input.RespectTtlEnabled ||
                    (this.RespectTtlEnabled != null &&
                    this.RespectTtlEnabled.Equals(input.RespectTtlEnabled))
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
                if (this.AccessType != null)
                    hashCode = hashCode * 59 + this.AccessType.GetHashCode();
                if (this.ConsumerAckPropagationEnabled != null)
                    hashCode = hashCode * 59 + this.ConsumerAckPropagationEnabled.GetHashCode();
                if (this.DeadMsgQueue != null)
                    hashCode = hashCode * 59 + this.DeadMsgQueue.GetHashCode();
                if (this.DeliveryDelay != null)
                    hashCode = hashCode * 59 + this.DeliveryDelay.GetHashCode();
                if (this.DurabilityOverride != null)
                    hashCode = hashCode * 59 + this.DurabilityOverride.GetHashCode();
                if (this.EventBindCountThreshold != null)
                    hashCode = hashCode * 59 + this.EventBindCountThreshold.GetHashCode();
                if (this.EventMsgSpoolUsageThreshold != null)
                    hashCode = hashCode * 59 + this.EventMsgSpoolUsageThreshold.GetHashCode();
                if (this.EventRejectLowPriorityMsgLimitThreshold != null)
                    hashCode = hashCode * 59 + this.EventRejectLowPriorityMsgLimitThreshold.GetHashCode();
                if (this.MaxBindCount != null)
                    hashCode = hashCode * 59 + this.MaxBindCount.GetHashCode();
                if (this.MaxDeliveredUnackedMsgsPerFlow != null)
                    hashCode = hashCode * 59 + this.MaxDeliveredUnackedMsgsPerFlow.GetHashCode();
                if (this.MaxMsgSize != null)
                    hashCode = hashCode * 59 + this.MaxMsgSize.GetHashCode();
                if (this.MaxMsgSpoolUsage != null)
                    hashCode = hashCode * 59 + this.MaxMsgSpoolUsage.GetHashCode();
                if (this.MaxRedeliveryCount != null)
                    hashCode = hashCode * 59 + this.MaxRedeliveryCount.GetHashCode();
                if (this.MaxTtl != null)
                    hashCode = hashCode * 59 + this.MaxTtl.GetHashCode();
                if (this.MsgVpnName != null)
                    hashCode = hashCode * 59 + this.MsgVpnName.GetHashCode();
                if (this.Permission != null)
                    hashCode = hashCode * 59 + this.Permission.GetHashCode();
                if (this.QueueNameFilter != null)
                    hashCode = hashCode * 59 + this.QueueNameFilter.GetHashCode();
                if (this.QueueTemplateName != null)
                    hashCode = hashCode * 59 + this.QueueTemplateName.GetHashCode();
                if (this.RedeliveryDelayEnabled != null)
                    hashCode = hashCode * 59 + this.RedeliveryDelayEnabled.GetHashCode();
                if (this.RedeliveryDelayInitialInterval != null)
                    hashCode = hashCode * 59 + this.RedeliveryDelayInitialInterval.GetHashCode();
                if (this.RedeliveryDelayMaxInterval != null)
                    hashCode = hashCode * 59 + this.RedeliveryDelayMaxInterval.GetHashCode();
                if (this.RedeliveryDelayMultiplier != null)
                    hashCode = hashCode * 59 + this.RedeliveryDelayMultiplier.GetHashCode();
                if (this.RedeliveryEnabled != null)
                    hashCode = hashCode * 59 + this.RedeliveryEnabled.GetHashCode();
                if (this.RejectLowPriorityMsgEnabled != null)
                    hashCode = hashCode * 59 + this.RejectLowPriorityMsgEnabled.GetHashCode();
                if (this.RejectLowPriorityMsgLimit != null)
                    hashCode = hashCode * 59 + this.RejectLowPriorityMsgLimit.GetHashCode();
                if (this.RejectMsgToSenderOnDiscardBehavior != null)
                    hashCode = hashCode * 59 + this.RejectMsgToSenderOnDiscardBehavior.GetHashCode();
                if (this.RespectMsgPriorityEnabled != null)
                    hashCode = hashCode * 59 + this.RespectMsgPriorityEnabled.GetHashCode();
                if (this.RespectTtlEnabled != null)
                    hashCode = hashCode * 59 + this.RespectTtlEnabled.GetHashCode();
                return hashCode;
            }
        }
    }
}
