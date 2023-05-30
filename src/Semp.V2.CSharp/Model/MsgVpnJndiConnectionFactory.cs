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
    /// MsgVpnJndiConnectionFactory
    /// </summary>
    [DataContract]
        public partial class MsgVpnJndiConnectionFactory :  IEquatable<MsgVpnJndiConnectionFactory>
    {
        /// <summary>
        /// The default delivery mode for messages sent by the Publisher (Producer). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;persistent\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;persistent\&quot; - The broker spools messages (persists in the Message Spool) as part of the send operation. \&quot;non-persistent\&quot; - The broker does not spool messages (does not persist in the Message Spool) as part of the send operation. &lt;/pre&gt; 
        /// </summary>
        /// <value>The default delivery mode for messages sent by the Publisher (Producer). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;persistent\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;persistent\&quot; - The broker spools messages (persists in the Message Spool) as part of the send operation. \&quot;non-persistent\&quot; - The broker does not spool messages (does not persist in the Message Spool) as part of the send operation. &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum MessagingDefaultDeliveryModeEnum
        {
            /// <summary>
            /// Enum Persistent for value: persistent
            /// </summary>
            [EnumMember(Value = "persistent")]
            Persistent = 1,
            /// <summary>
            /// Enum NonPersistent for value: non-persistent
            /// </summary>
            [EnumMember(Value = "non-persistent")]
            NonPersistent = 2        }
        /// <summary>
        /// The default delivery mode for messages sent by the Publisher (Producer). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;persistent\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;persistent\&quot; - The broker spools messages (persists in the Message Spool) as part of the send operation. \&quot;non-persistent\&quot; - The broker does not spool messages (does not persist in the Message Spool) as part of the send operation. &lt;/pre&gt; 
        /// </summary>
        /// <value>The default delivery mode for messages sent by the Publisher (Producer). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;persistent\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;persistent\&quot; - The broker spools messages (persists in the Message Spool) as part of the send operation. \&quot;non-persistent\&quot; - The broker does not spool messages (does not persist in the Message Spool) as part of the send operation. &lt;/pre&gt; </value>
        [DataMember(Name="messagingDefaultDeliveryMode", EmitDefaultValue=false)]
        public MessagingDefaultDeliveryModeEnum? MessagingDefaultDeliveryMode { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="MsgVpnJndiConnectionFactory" /> class.
        /// </summary>
        /// <param name="allowDuplicateClientIdEnabled">Enable or disable whether new JMS connections can use the same Client identifier (ID) as an existing connection. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.3..</param>
        /// <param name="clientDescription">The description of the Client. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="clientId">The Client identifier (ID). If not specified, a unique value for it will be generated. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="connectionFactoryName">The name of the JMS Connection Factory..</param>
        /// <param name="dtoReceiveOverrideEnabled">Enable or disable overriding by the Subscriber (Consumer) of the deliver-to-one (DTO) property on messages. When enabled, the Subscriber can receive all DTO tagged messages. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="dtoReceiveSubscriberLocalPriority">The priority for receiving deliver-to-one (DTO) messages by the Subscriber (Consumer) if the messages are published on the local broker that the Subscriber is directly connected to. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1&#x60;..</param>
        /// <param name="dtoReceiveSubscriberNetworkPriority">The priority for receiving deliver-to-one (DTO) messages by the Subscriber (Consumer) if the messages are published on a remote broker. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1&#x60;..</param>
        /// <param name="dtoSendEnabled">Enable or disable the deliver-to-one (DTO) property on messages sent by the Publisher (Producer). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="dynamicEndpointCreateDurableEnabled">Enable or disable whether a durable endpoint will be dynamically created on the broker when the client calls \&quot;Session.createDurableSubscriber()\&quot; or \&quot;Session.createQueue()\&quot;. The created endpoint respects the message time-to-live (TTL) according to the \&quot;dynamicEndpointRespectTtlEnabled\&quot; property. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="dynamicEndpointRespectTtlEnabled">Enable or disable whether dynamically created durable and non-durable endpoints respect the message time-to-live (TTL) property. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="guaranteedReceiveAckTimeout">The timeout for sending the acknowledgement (ACK) for guaranteed messages received by the Subscriber (Consumer), in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;..</param>
        /// <param name="guaranteedReceiveReconnectRetryCount">The maximum number of attempts to reconnect to the host or list of hosts after the guaranteed  messaging connection has been lost. The value \&quot;-1\&quot; means to retry forever. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;-1&#x60;. Available since 2.14..</param>
        /// <param name="guaranteedReceiveReconnectRetryWait">The amount of time to wait before making another attempt to connect or reconnect to the host after the guaranteed messaging connection has been lost, in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3000&#x60;. Available since 2.14..</param>
        /// <param name="guaranteedReceiveWindowSize">The size of the window for guaranteed messages received by the Subscriber (Consumer), in messages. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;18&#x60;..</param>
        /// <param name="guaranteedReceiveWindowSizeAckThreshold">The threshold for sending the acknowledgement (ACK) for guaranteed messages received by the Subscriber (Consumer) as a percentage of &#x60;guaranteedReceiveWindowSize&#x60;. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;60&#x60;..</param>
        /// <param name="guaranteedSendAckTimeout">The timeout for receiving the acknowledgement (ACK) for guaranteed messages sent by the Publisher (Producer), in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;2000&#x60;..</param>
        /// <param name="guaranteedSendWindowSize">The size of the window for non-persistent guaranteed messages sent by the Publisher (Producer), in messages. For persistent messages the window size is fixed at 1. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;255&#x60;..</param>
        /// <param name="messagingDefaultDeliveryMode">The default delivery mode for messages sent by the Publisher (Producer). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;persistent\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;persistent\&quot; - The broker spools messages (persists in the Message Spool) as part of the send operation. \&quot;non-persistent\&quot; - The broker does not spool messages (does not persist in the Message Spool) as part of the send operation. &lt;/pre&gt; .</param>
        /// <param name="messagingDefaultDmqEligibleEnabled">Enable or disable whether messages sent by the Publisher (Producer) are Dead Message Queue (DMQ) eligible by default. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="messagingDefaultElidingEligibleEnabled">Enable or disable whether messages sent by the Publisher (Producer) are Eliding eligible by default. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="messagingJmsxUserIdEnabled">Enable or disable inclusion (adding or replacing) of the JMSXUserID property in messages sent by the Publisher (Producer). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="messagingTextInXmlPayloadEnabled">Enable or disable encoding of JMS text messages in Publisher (Producer) messages as XML payload. When disabled, JMS text messages are encoded as a binary attachment. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="msgVpnName">The name of the Message VPN..</param>
        /// <param name="transportCompressionLevel">The ZLIB compression level for the connection to the broker. The value \&quot;0\&quot; means no compression, and the value \&quot;-1\&quot; means the compression level is specified in the JNDI Properties file. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;-1&#x60;..</param>
        /// <param name="transportConnectRetryCount">The maximum number of retry attempts to establish an initial connection to the host or list of hosts. The value \&quot;0\&quot; means a single attempt (no retries), and the value \&quot;-1\&quot; means to retry forever. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;..</param>
        /// <param name="transportConnectRetryPerHostCount">The maximum number of retry attempts to establish an initial connection to each host on the list of hosts. The value \&quot;0\&quot; means a single attempt (no retries), and the value \&quot;-1\&quot; means to retry forever. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;..</param>
        /// <param name="transportConnectTimeout">The timeout for establishing an initial connection to the broker, in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;30000&#x60;..</param>
        /// <param name="transportDirectTransportEnabled">Enable or disable usage of Direct Transport mode. When enabled, NON-PERSISTENT messages are sent as direct messages and non-durable topic consumers and temporary queue consumers consume using direct subscriptions rather than from guaranteed endpoints. If disabled all messaging uses guaranteed transport. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="transportKeepaliveCount">The maximum number of consecutive application-level keepalive messages sent without the broker response before the connection to the broker is closed. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;..</param>
        /// <param name="transportKeepaliveEnabled">Enable or disable usage of application-level keepalive messages to maintain a connection with the broker. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="transportKeepaliveInterval">The interval between application-level keepalive messages, in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3000&#x60;..</param>
        /// <param name="transportMsgCallbackOnIoThreadEnabled">Enable or disable delivery of asynchronous messages directly from the I/O thread. Contact support before enabling this property. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="transportOptimizeDirectEnabled">Enable or disable optimization for the Direct Transport delivery mode. If enabled, the client application is limited to one Publisher (Producer) and one non-durable Subscriber (Consumer). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="transportPort">The connection port number on the broker for SMF clients. The value \&quot;-1\&quot; means the port is specified in the JNDI Properties file. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;-1&#x60;..</param>
        /// <param name="transportReadTimeout">The timeout for reading a reply from the broker, in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;10000&#x60;..</param>
        /// <param name="transportReceiveBufferSize">The size of the receive socket buffer, in bytes. It corresponds to the SO_RCVBUF socket option. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;65536&#x60;..</param>
        /// <param name="transportReconnectRetryCount">The maximum number of attempts to reconnect to the host or list of hosts after the connection has been lost. The value \&quot;-1\&quot; means to retry forever. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;..</param>
        /// <param name="transportReconnectRetryWait">The amount of time before making another attempt to connect or reconnect to the host after the connection has been lost, in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3000&#x60;..</param>
        /// <param name="transportSendBufferSize">The size of the send socket buffer, in bytes. It corresponds to the SO_SNDBUF socket option. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;65536&#x60;..</param>
        /// <param name="transportTcpNoDelayEnabled">Enable or disable the TCP_NODELAY option. When enabled, Nagle&#x27;s algorithm for TCP/IP congestion control (RFC 896) is disabled. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="xaEnabled">Enable or disable this as an XA Connection Factory. When enabled, the Connection Factory can be cast to \&quot;XAConnectionFactory\&quot;, \&quot;XAQueueConnectionFactory\&quot; or \&quot;XATopicConnectionFactory\&quot;. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        public MsgVpnJndiConnectionFactory(bool? allowDuplicateClientIdEnabled = default(bool?), string clientDescription = default(string), string clientId = default(string), string connectionFactoryName = default(string), bool? dtoReceiveOverrideEnabled = default(bool?), int? dtoReceiveSubscriberLocalPriority = default(int?), int? dtoReceiveSubscriberNetworkPriority = default(int?), bool? dtoSendEnabled = default(bool?), bool? dynamicEndpointCreateDurableEnabled = default(bool?), bool? dynamicEndpointRespectTtlEnabled = default(bool?), int? guaranteedReceiveAckTimeout = default(int?), int? guaranteedReceiveReconnectRetryCount = default(int?), int? guaranteedReceiveReconnectRetryWait = default(int?), int? guaranteedReceiveWindowSize = default(int?), int? guaranteedReceiveWindowSizeAckThreshold = default(int?), int? guaranteedSendAckTimeout = default(int?), int? guaranteedSendWindowSize = default(int?), MessagingDefaultDeliveryModeEnum? messagingDefaultDeliveryMode = default(MessagingDefaultDeliveryModeEnum?), bool? messagingDefaultDmqEligibleEnabled = default(bool?), bool? messagingDefaultElidingEligibleEnabled = default(bool?), bool? messagingJmsxUserIdEnabled = default(bool?), bool? messagingTextInXmlPayloadEnabled = default(bool?), string msgVpnName = default(string), int? transportCompressionLevel = default(int?), int? transportConnectRetryCount = default(int?), int? transportConnectRetryPerHostCount = default(int?), int? transportConnectTimeout = default(int?), bool? transportDirectTransportEnabled = default(bool?), int? transportKeepaliveCount = default(int?), bool? transportKeepaliveEnabled = default(bool?), int? transportKeepaliveInterval = default(int?), bool? transportMsgCallbackOnIoThreadEnabled = default(bool?), bool? transportOptimizeDirectEnabled = default(bool?), int? transportPort = default(int?), int? transportReadTimeout = default(int?), int? transportReceiveBufferSize = default(int?), int? transportReconnectRetryCount = default(int?), int? transportReconnectRetryWait = default(int?), int? transportSendBufferSize = default(int?), bool? transportTcpNoDelayEnabled = default(bool?), bool? xaEnabled = default(bool?))
        {
            this.AllowDuplicateClientIdEnabled = allowDuplicateClientIdEnabled;
            this.ClientDescription = clientDescription;
            this.ClientId = clientId;
            this.ConnectionFactoryName = connectionFactoryName;
            this.DtoReceiveOverrideEnabled = dtoReceiveOverrideEnabled;
            this.DtoReceiveSubscriberLocalPriority = dtoReceiveSubscriberLocalPriority;
            this.DtoReceiveSubscriberNetworkPriority = dtoReceiveSubscriberNetworkPriority;
            this.DtoSendEnabled = dtoSendEnabled;
            this.DynamicEndpointCreateDurableEnabled = dynamicEndpointCreateDurableEnabled;
            this.DynamicEndpointRespectTtlEnabled = dynamicEndpointRespectTtlEnabled;
            this.GuaranteedReceiveAckTimeout = guaranteedReceiveAckTimeout;
            this.GuaranteedReceiveReconnectRetryCount = guaranteedReceiveReconnectRetryCount;
            this.GuaranteedReceiveReconnectRetryWait = guaranteedReceiveReconnectRetryWait;
            this.GuaranteedReceiveWindowSize = guaranteedReceiveWindowSize;
            this.GuaranteedReceiveWindowSizeAckThreshold = guaranteedReceiveWindowSizeAckThreshold;
            this.GuaranteedSendAckTimeout = guaranteedSendAckTimeout;
            this.GuaranteedSendWindowSize = guaranteedSendWindowSize;
            this.MessagingDefaultDeliveryMode = messagingDefaultDeliveryMode;
            this.MessagingDefaultDmqEligibleEnabled = messagingDefaultDmqEligibleEnabled;
            this.MessagingDefaultElidingEligibleEnabled = messagingDefaultElidingEligibleEnabled;
            this.MessagingJmsxUserIdEnabled = messagingJmsxUserIdEnabled;
            this.MessagingTextInXmlPayloadEnabled = messagingTextInXmlPayloadEnabled;
            this.MsgVpnName = msgVpnName;
            this.TransportCompressionLevel = transportCompressionLevel;
            this.TransportConnectRetryCount = transportConnectRetryCount;
            this.TransportConnectRetryPerHostCount = transportConnectRetryPerHostCount;
            this.TransportConnectTimeout = transportConnectTimeout;
            this.TransportDirectTransportEnabled = transportDirectTransportEnabled;
            this.TransportKeepaliveCount = transportKeepaliveCount;
            this.TransportKeepaliveEnabled = transportKeepaliveEnabled;
            this.TransportKeepaliveInterval = transportKeepaliveInterval;
            this.TransportMsgCallbackOnIoThreadEnabled = transportMsgCallbackOnIoThreadEnabled;
            this.TransportOptimizeDirectEnabled = transportOptimizeDirectEnabled;
            this.TransportPort = transportPort;
            this.TransportReadTimeout = transportReadTimeout;
            this.TransportReceiveBufferSize = transportReceiveBufferSize;
            this.TransportReconnectRetryCount = transportReconnectRetryCount;
            this.TransportReconnectRetryWait = transportReconnectRetryWait;
            this.TransportSendBufferSize = transportSendBufferSize;
            this.TransportTcpNoDelayEnabled = transportTcpNoDelayEnabled;
            this.XaEnabled = xaEnabled;
        }
        
        /// <summary>
        /// Enable or disable whether new JMS connections can use the same Client identifier (ID) as an existing connection. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.3.
        /// </summary>
        /// <value>Enable or disable whether new JMS connections can use the same Client identifier (ID) as an existing connection. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.3.</value>
        [DataMember(Name="allowDuplicateClientIdEnabled", EmitDefaultValue=false)]
        public bool? AllowDuplicateClientIdEnabled { get; set; }

        /// <summary>
        /// The description of the Client. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The description of the Client. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="clientDescription", EmitDefaultValue=false)]
        public string ClientDescription { get; set; }

        /// <summary>
        /// The Client identifier (ID). If not specified, a unique value for it will be generated. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The Client identifier (ID). If not specified, a unique value for it will be generated. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="clientId", EmitDefaultValue=false)]
        public string ClientId { get; set; }

        /// <summary>
        /// The name of the JMS Connection Factory.
        /// </summary>
        /// <value>The name of the JMS Connection Factory.</value>
        [DataMember(Name="connectionFactoryName", EmitDefaultValue=false)]
        public string ConnectionFactoryName { get; set; }

        /// <summary>
        /// Enable or disable overriding by the Subscriber (Consumer) of the deliver-to-one (DTO) property on messages. When enabled, the Subscriber can receive all DTO tagged messages. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable overriding by the Subscriber (Consumer) of the deliver-to-one (DTO) property on messages. When enabled, the Subscriber can receive all DTO tagged messages. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="dtoReceiveOverrideEnabled", EmitDefaultValue=false)]
        public bool? DtoReceiveOverrideEnabled { get; set; }

        /// <summary>
        /// The priority for receiving deliver-to-one (DTO) messages by the Subscriber (Consumer) if the messages are published on the local broker that the Subscriber is directly connected to. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1&#x60;.
        /// </summary>
        /// <value>The priority for receiving deliver-to-one (DTO) messages by the Subscriber (Consumer) if the messages are published on the local broker that the Subscriber is directly connected to. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1&#x60;.</value>
        [DataMember(Name="dtoReceiveSubscriberLocalPriority", EmitDefaultValue=false)]
        public int? DtoReceiveSubscriberLocalPriority { get; set; }

        /// <summary>
        /// The priority for receiving deliver-to-one (DTO) messages by the Subscriber (Consumer) if the messages are published on a remote broker. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1&#x60;.
        /// </summary>
        /// <value>The priority for receiving deliver-to-one (DTO) messages by the Subscriber (Consumer) if the messages are published on a remote broker. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1&#x60;.</value>
        [DataMember(Name="dtoReceiveSubscriberNetworkPriority", EmitDefaultValue=false)]
        public int? DtoReceiveSubscriberNetworkPriority { get; set; }

        /// <summary>
        /// Enable or disable the deliver-to-one (DTO) property on messages sent by the Publisher (Producer). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable the deliver-to-one (DTO) property on messages sent by the Publisher (Producer). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="dtoSendEnabled", EmitDefaultValue=false)]
        public bool? DtoSendEnabled { get; set; }

        /// <summary>
        /// Enable or disable whether a durable endpoint will be dynamically created on the broker when the client calls \&quot;Session.createDurableSubscriber()\&quot; or \&quot;Session.createQueue()\&quot;. The created endpoint respects the message time-to-live (TTL) according to the \&quot;dynamicEndpointRespectTtlEnabled\&quot; property. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable whether a durable endpoint will be dynamically created on the broker when the client calls \&quot;Session.createDurableSubscriber()\&quot; or \&quot;Session.createQueue()\&quot;. The created endpoint respects the message time-to-live (TTL) according to the \&quot;dynamicEndpointRespectTtlEnabled\&quot; property. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="dynamicEndpointCreateDurableEnabled", EmitDefaultValue=false)]
        public bool? DynamicEndpointCreateDurableEnabled { get; set; }

        /// <summary>
        /// Enable or disable whether dynamically created durable and non-durable endpoints respect the message time-to-live (TTL) property. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable whether dynamically created durable and non-durable endpoints respect the message time-to-live (TTL) property. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="dynamicEndpointRespectTtlEnabled", EmitDefaultValue=false)]
        public bool? DynamicEndpointRespectTtlEnabled { get; set; }

        /// <summary>
        /// The timeout for sending the acknowledgement (ACK) for guaranteed messages received by the Subscriber (Consumer), in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;.
        /// </summary>
        /// <value>The timeout for sending the acknowledgement (ACK) for guaranteed messages received by the Subscriber (Consumer), in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;.</value>
        [DataMember(Name="guaranteedReceiveAckTimeout", EmitDefaultValue=false)]
        public int? GuaranteedReceiveAckTimeout { get; set; }

        /// <summary>
        /// The maximum number of attempts to reconnect to the host or list of hosts after the guaranteed  messaging connection has been lost. The value \&quot;-1\&quot; means to retry forever. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;-1&#x60;. Available since 2.14.
        /// </summary>
        /// <value>The maximum number of attempts to reconnect to the host or list of hosts after the guaranteed  messaging connection has been lost. The value \&quot;-1\&quot; means to retry forever. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;-1&#x60;. Available since 2.14.</value>
        [DataMember(Name="guaranteedReceiveReconnectRetryCount", EmitDefaultValue=false)]
        public int? GuaranteedReceiveReconnectRetryCount { get; set; }

        /// <summary>
        /// The amount of time to wait before making another attempt to connect or reconnect to the host after the guaranteed messaging connection has been lost, in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3000&#x60;. Available since 2.14.
        /// </summary>
        /// <value>The amount of time to wait before making another attempt to connect or reconnect to the host after the guaranteed messaging connection has been lost, in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3000&#x60;. Available since 2.14.</value>
        [DataMember(Name="guaranteedReceiveReconnectRetryWait", EmitDefaultValue=false)]
        public int? GuaranteedReceiveReconnectRetryWait { get; set; }

        /// <summary>
        /// The size of the window for guaranteed messages received by the Subscriber (Consumer), in messages. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;18&#x60;.
        /// </summary>
        /// <value>The size of the window for guaranteed messages received by the Subscriber (Consumer), in messages. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;18&#x60;.</value>
        [DataMember(Name="guaranteedReceiveWindowSize", EmitDefaultValue=false)]
        public int? GuaranteedReceiveWindowSize { get; set; }

        /// <summary>
        /// The threshold for sending the acknowledgement (ACK) for guaranteed messages received by the Subscriber (Consumer) as a percentage of &#x60;guaranteedReceiveWindowSize&#x60;. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;60&#x60;.
        /// </summary>
        /// <value>The threshold for sending the acknowledgement (ACK) for guaranteed messages received by the Subscriber (Consumer) as a percentage of &#x60;guaranteedReceiveWindowSize&#x60;. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;60&#x60;.</value>
        [DataMember(Name="guaranteedReceiveWindowSizeAckThreshold", EmitDefaultValue=false)]
        public int? GuaranteedReceiveWindowSizeAckThreshold { get; set; }

        /// <summary>
        /// The timeout for receiving the acknowledgement (ACK) for guaranteed messages sent by the Publisher (Producer), in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;2000&#x60;.
        /// </summary>
        /// <value>The timeout for receiving the acknowledgement (ACK) for guaranteed messages sent by the Publisher (Producer), in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;2000&#x60;.</value>
        [DataMember(Name="guaranteedSendAckTimeout", EmitDefaultValue=false)]
        public int? GuaranteedSendAckTimeout { get; set; }

        /// <summary>
        /// The size of the window for non-persistent guaranteed messages sent by the Publisher (Producer), in messages. For persistent messages the window size is fixed at 1. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;255&#x60;.
        /// </summary>
        /// <value>The size of the window for non-persistent guaranteed messages sent by the Publisher (Producer), in messages. For persistent messages the window size is fixed at 1. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;255&#x60;.</value>
        [DataMember(Name="guaranteedSendWindowSize", EmitDefaultValue=false)]
        public int? GuaranteedSendWindowSize { get; set; }


        /// <summary>
        /// Enable or disable whether messages sent by the Publisher (Producer) are Dead Message Queue (DMQ) eligible by default. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable whether messages sent by the Publisher (Producer) are Dead Message Queue (DMQ) eligible by default. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="messagingDefaultDmqEligibleEnabled", EmitDefaultValue=false)]
        public bool? MessagingDefaultDmqEligibleEnabled { get; set; }

        /// <summary>
        /// Enable or disable whether messages sent by the Publisher (Producer) are Eliding eligible by default. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable whether messages sent by the Publisher (Producer) are Eliding eligible by default. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="messagingDefaultElidingEligibleEnabled", EmitDefaultValue=false)]
        public bool? MessagingDefaultElidingEligibleEnabled { get; set; }

        /// <summary>
        /// Enable or disable inclusion (adding or replacing) of the JMSXUserID property in messages sent by the Publisher (Producer). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable inclusion (adding or replacing) of the JMSXUserID property in messages sent by the Publisher (Producer). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="messagingJmsxUserIdEnabled", EmitDefaultValue=false)]
        public bool? MessagingJmsxUserIdEnabled { get; set; }

        /// <summary>
        /// Enable or disable encoding of JMS text messages in Publisher (Producer) messages as XML payload. When disabled, JMS text messages are encoded as a binary attachment. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable encoding of JMS text messages in Publisher (Producer) messages as XML payload. When disabled, JMS text messages are encoded as a binary attachment. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="messagingTextInXmlPayloadEnabled", EmitDefaultValue=false)]
        public bool? MessagingTextInXmlPayloadEnabled { get; set; }

        /// <summary>
        /// The name of the Message VPN.
        /// </summary>
        /// <value>The name of the Message VPN.</value>
        [DataMember(Name="msgVpnName", EmitDefaultValue=false)]
        public string MsgVpnName { get; set; }

        /// <summary>
        /// The ZLIB compression level for the connection to the broker. The value \&quot;0\&quot; means no compression, and the value \&quot;-1\&quot; means the compression level is specified in the JNDI Properties file. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;-1&#x60;.
        /// </summary>
        /// <value>The ZLIB compression level for the connection to the broker. The value \&quot;0\&quot; means no compression, and the value \&quot;-1\&quot; means the compression level is specified in the JNDI Properties file. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;-1&#x60;.</value>
        [DataMember(Name="transportCompressionLevel", EmitDefaultValue=false)]
        public int? TransportCompressionLevel { get; set; }

        /// <summary>
        /// The maximum number of retry attempts to establish an initial connection to the host or list of hosts. The value \&quot;0\&quot; means a single attempt (no retries), and the value \&quot;-1\&quot; means to retry forever. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;.
        /// </summary>
        /// <value>The maximum number of retry attempts to establish an initial connection to the host or list of hosts. The value \&quot;0\&quot; means a single attempt (no retries), and the value \&quot;-1\&quot; means to retry forever. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;.</value>
        [DataMember(Name="transportConnectRetryCount", EmitDefaultValue=false)]
        public int? TransportConnectRetryCount { get; set; }

        /// <summary>
        /// The maximum number of retry attempts to establish an initial connection to each host on the list of hosts. The value \&quot;0\&quot; means a single attempt (no retries), and the value \&quot;-1\&quot; means to retry forever. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;.
        /// </summary>
        /// <value>The maximum number of retry attempts to establish an initial connection to each host on the list of hosts. The value \&quot;0\&quot; means a single attempt (no retries), and the value \&quot;-1\&quot; means to retry forever. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;.</value>
        [DataMember(Name="transportConnectRetryPerHostCount", EmitDefaultValue=false)]
        public int? TransportConnectRetryPerHostCount { get; set; }

        /// <summary>
        /// The timeout for establishing an initial connection to the broker, in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;30000&#x60;.
        /// </summary>
        /// <value>The timeout for establishing an initial connection to the broker, in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;30000&#x60;.</value>
        [DataMember(Name="transportConnectTimeout", EmitDefaultValue=false)]
        public int? TransportConnectTimeout { get; set; }

        /// <summary>
        /// Enable or disable usage of Direct Transport mode. When enabled, NON-PERSISTENT messages are sent as direct messages and non-durable topic consumers and temporary queue consumers consume using direct subscriptions rather than from guaranteed endpoints. If disabled all messaging uses guaranteed transport. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable usage of Direct Transport mode. When enabled, NON-PERSISTENT messages are sent as direct messages and non-durable topic consumers and temporary queue consumers consume using direct subscriptions rather than from guaranteed endpoints. If disabled all messaging uses guaranteed transport. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="transportDirectTransportEnabled", EmitDefaultValue=false)]
        public bool? TransportDirectTransportEnabled { get; set; }

        /// <summary>
        /// The maximum number of consecutive application-level keepalive messages sent without the broker response before the connection to the broker is closed. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;.
        /// </summary>
        /// <value>The maximum number of consecutive application-level keepalive messages sent without the broker response before the connection to the broker is closed. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;.</value>
        [DataMember(Name="transportKeepaliveCount", EmitDefaultValue=false)]
        public int? TransportKeepaliveCount { get; set; }

        /// <summary>
        /// Enable or disable usage of application-level keepalive messages to maintain a connection with the broker. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable usage of application-level keepalive messages to maintain a connection with the broker. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="transportKeepaliveEnabled", EmitDefaultValue=false)]
        public bool? TransportKeepaliveEnabled { get; set; }

        /// <summary>
        /// The interval between application-level keepalive messages, in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3000&#x60;.
        /// </summary>
        /// <value>The interval between application-level keepalive messages, in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3000&#x60;.</value>
        [DataMember(Name="transportKeepaliveInterval", EmitDefaultValue=false)]
        public int? TransportKeepaliveInterval { get; set; }

        /// <summary>
        /// Enable or disable delivery of asynchronous messages directly from the I/O thread. Contact support before enabling this property. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable delivery of asynchronous messages directly from the I/O thread. Contact support before enabling this property. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="transportMsgCallbackOnIoThreadEnabled", EmitDefaultValue=false)]
        public bool? TransportMsgCallbackOnIoThreadEnabled { get; set; }

        /// <summary>
        /// Enable or disable optimization for the Direct Transport delivery mode. If enabled, the client application is limited to one Publisher (Producer) and one non-durable Subscriber (Consumer). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable optimization for the Direct Transport delivery mode. If enabled, the client application is limited to one Publisher (Producer) and one non-durable Subscriber (Consumer). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="transportOptimizeDirectEnabled", EmitDefaultValue=false)]
        public bool? TransportOptimizeDirectEnabled { get; set; }

        /// <summary>
        /// The connection port number on the broker for SMF clients. The value \&quot;-1\&quot; means the port is specified in the JNDI Properties file. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;-1&#x60;.
        /// </summary>
        /// <value>The connection port number on the broker for SMF clients. The value \&quot;-1\&quot; means the port is specified in the JNDI Properties file. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;-1&#x60;.</value>
        [DataMember(Name="transportPort", EmitDefaultValue=false)]
        public int? TransportPort { get; set; }

        /// <summary>
        /// The timeout for reading a reply from the broker, in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;10000&#x60;.
        /// </summary>
        /// <value>The timeout for reading a reply from the broker, in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;10000&#x60;.</value>
        [DataMember(Name="transportReadTimeout", EmitDefaultValue=false)]
        public int? TransportReadTimeout { get; set; }

        /// <summary>
        /// The size of the receive socket buffer, in bytes. It corresponds to the SO_RCVBUF socket option. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;65536&#x60;.
        /// </summary>
        /// <value>The size of the receive socket buffer, in bytes. It corresponds to the SO_RCVBUF socket option. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;65536&#x60;.</value>
        [DataMember(Name="transportReceiveBufferSize", EmitDefaultValue=false)]
        public int? TransportReceiveBufferSize { get; set; }

        /// <summary>
        /// The maximum number of attempts to reconnect to the host or list of hosts after the connection has been lost. The value \&quot;-1\&quot; means to retry forever. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;.
        /// </summary>
        /// <value>The maximum number of attempts to reconnect to the host or list of hosts after the connection has been lost. The value \&quot;-1\&quot; means to retry forever. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;.</value>
        [DataMember(Name="transportReconnectRetryCount", EmitDefaultValue=false)]
        public int? TransportReconnectRetryCount { get; set; }

        /// <summary>
        /// The amount of time before making another attempt to connect or reconnect to the host after the connection has been lost, in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3000&#x60;.
        /// </summary>
        /// <value>The amount of time before making another attempt to connect or reconnect to the host after the connection has been lost, in milliseconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3000&#x60;.</value>
        [DataMember(Name="transportReconnectRetryWait", EmitDefaultValue=false)]
        public int? TransportReconnectRetryWait { get; set; }

        /// <summary>
        /// The size of the send socket buffer, in bytes. It corresponds to the SO_SNDBUF socket option. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;65536&#x60;.
        /// </summary>
        /// <value>The size of the send socket buffer, in bytes. It corresponds to the SO_SNDBUF socket option. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;65536&#x60;.</value>
        [DataMember(Name="transportSendBufferSize", EmitDefaultValue=false)]
        public int? TransportSendBufferSize { get; set; }

        /// <summary>
        /// Enable or disable the TCP_NODELAY option. When enabled, Nagle&#x27;s algorithm for TCP/IP congestion control (RFC 896) is disabled. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable the TCP_NODELAY option. When enabled, Nagle&#x27;s algorithm for TCP/IP congestion control (RFC 896) is disabled. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="transportTcpNoDelayEnabled", EmitDefaultValue=false)]
        public bool? TransportTcpNoDelayEnabled { get; set; }

        /// <summary>
        /// Enable or disable this as an XA Connection Factory. When enabled, the Connection Factory can be cast to \&quot;XAConnectionFactory\&quot;, \&quot;XAQueueConnectionFactory\&quot; or \&quot;XATopicConnectionFactory\&quot;. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable this as an XA Connection Factory. When enabled, the Connection Factory can be cast to \&quot;XAConnectionFactory\&quot;, \&quot;XAQueueConnectionFactory\&quot; or \&quot;XATopicConnectionFactory\&quot;. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="xaEnabled", EmitDefaultValue=false)]
        public bool? XaEnabled { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class MsgVpnJndiConnectionFactory {\n");
            sb.Append("  AllowDuplicateClientIdEnabled: ").Append(AllowDuplicateClientIdEnabled).Append("\n");
            sb.Append("  ClientDescription: ").Append(ClientDescription).Append("\n");
            sb.Append("  ClientId: ").Append(ClientId).Append("\n");
            sb.Append("  ConnectionFactoryName: ").Append(ConnectionFactoryName).Append("\n");
            sb.Append("  DtoReceiveOverrideEnabled: ").Append(DtoReceiveOverrideEnabled).Append("\n");
            sb.Append("  DtoReceiveSubscriberLocalPriority: ").Append(DtoReceiveSubscriberLocalPriority).Append("\n");
            sb.Append("  DtoReceiveSubscriberNetworkPriority: ").Append(DtoReceiveSubscriberNetworkPriority).Append("\n");
            sb.Append("  DtoSendEnabled: ").Append(DtoSendEnabled).Append("\n");
            sb.Append("  DynamicEndpointCreateDurableEnabled: ").Append(DynamicEndpointCreateDurableEnabled).Append("\n");
            sb.Append("  DynamicEndpointRespectTtlEnabled: ").Append(DynamicEndpointRespectTtlEnabled).Append("\n");
            sb.Append("  GuaranteedReceiveAckTimeout: ").Append(GuaranteedReceiveAckTimeout).Append("\n");
            sb.Append("  GuaranteedReceiveReconnectRetryCount: ").Append(GuaranteedReceiveReconnectRetryCount).Append("\n");
            sb.Append("  GuaranteedReceiveReconnectRetryWait: ").Append(GuaranteedReceiveReconnectRetryWait).Append("\n");
            sb.Append("  GuaranteedReceiveWindowSize: ").Append(GuaranteedReceiveWindowSize).Append("\n");
            sb.Append("  GuaranteedReceiveWindowSizeAckThreshold: ").Append(GuaranteedReceiveWindowSizeAckThreshold).Append("\n");
            sb.Append("  GuaranteedSendAckTimeout: ").Append(GuaranteedSendAckTimeout).Append("\n");
            sb.Append("  GuaranteedSendWindowSize: ").Append(GuaranteedSendWindowSize).Append("\n");
            sb.Append("  MessagingDefaultDeliveryMode: ").Append(MessagingDefaultDeliveryMode).Append("\n");
            sb.Append("  MessagingDefaultDmqEligibleEnabled: ").Append(MessagingDefaultDmqEligibleEnabled).Append("\n");
            sb.Append("  MessagingDefaultElidingEligibleEnabled: ").Append(MessagingDefaultElidingEligibleEnabled).Append("\n");
            sb.Append("  MessagingJmsxUserIdEnabled: ").Append(MessagingJmsxUserIdEnabled).Append("\n");
            sb.Append("  MessagingTextInXmlPayloadEnabled: ").Append(MessagingTextInXmlPayloadEnabled).Append("\n");
            sb.Append("  MsgVpnName: ").Append(MsgVpnName).Append("\n");
            sb.Append("  TransportCompressionLevel: ").Append(TransportCompressionLevel).Append("\n");
            sb.Append("  TransportConnectRetryCount: ").Append(TransportConnectRetryCount).Append("\n");
            sb.Append("  TransportConnectRetryPerHostCount: ").Append(TransportConnectRetryPerHostCount).Append("\n");
            sb.Append("  TransportConnectTimeout: ").Append(TransportConnectTimeout).Append("\n");
            sb.Append("  TransportDirectTransportEnabled: ").Append(TransportDirectTransportEnabled).Append("\n");
            sb.Append("  TransportKeepaliveCount: ").Append(TransportKeepaliveCount).Append("\n");
            sb.Append("  TransportKeepaliveEnabled: ").Append(TransportKeepaliveEnabled).Append("\n");
            sb.Append("  TransportKeepaliveInterval: ").Append(TransportKeepaliveInterval).Append("\n");
            sb.Append("  TransportMsgCallbackOnIoThreadEnabled: ").Append(TransportMsgCallbackOnIoThreadEnabled).Append("\n");
            sb.Append("  TransportOptimizeDirectEnabled: ").Append(TransportOptimizeDirectEnabled).Append("\n");
            sb.Append("  TransportPort: ").Append(TransportPort).Append("\n");
            sb.Append("  TransportReadTimeout: ").Append(TransportReadTimeout).Append("\n");
            sb.Append("  TransportReceiveBufferSize: ").Append(TransportReceiveBufferSize).Append("\n");
            sb.Append("  TransportReconnectRetryCount: ").Append(TransportReconnectRetryCount).Append("\n");
            sb.Append("  TransportReconnectRetryWait: ").Append(TransportReconnectRetryWait).Append("\n");
            sb.Append("  TransportSendBufferSize: ").Append(TransportSendBufferSize).Append("\n");
            sb.Append("  TransportTcpNoDelayEnabled: ").Append(TransportTcpNoDelayEnabled).Append("\n");
            sb.Append("  XaEnabled: ").Append(XaEnabled).Append("\n");
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
            return this.Equals(input as MsgVpnJndiConnectionFactory);
        }

        /// <summary>
        /// Returns true if MsgVpnJndiConnectionFactory instances are equal
        /// </summary>
        /// <param name="input">Instance of MsgVpnJndiConnectionFactory to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(MsgVpnJndiConnectionFactory input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.AllowDuplicateClientIdEnabled == input.AllowDuplicateClientIdEnabled ||
                    (this.AllowDuplicateClientIdEnabled != null &&
                    this.AllowDuplicateClientIdEnabled.Equals(input.AllowDuplicateClientIdEnabled))
                ) && 
                (
                    this.ClientDescription == input.ClientDescription ||
                    (this.ClientDescription != null &&
                    this.ClientDescription.Equals(input.ClientDescription))
                ) && 
                (
                    this.ClientId == input.ClientId ||
                    (this.ClientId != null &&
                    this.ClientId.Equals(input.ClientId))
                ) && 
                (
                    this.ConnectionFactoryName == input.ConnectionFactoryName ||
                    (this.ConnectionFactoryName != null &&
                    this.ConnectionFactoryName.Equals(input.ConnectionFactoryName))
                ) && 
                (
                    this.DtoReceiveOverrideEnabled == input.DtoReceiveOverrideEnabled ||
                    (this.DtoReceiveOverrideEnabled != null &&
                    this.DtoReceiveOverrideEnabled.Equals(input.DtoReceiveOverrideEnabled))
                ) && 
                (
                    this.DtoReceiveSubscriberLocalPriority == input.DtoReceiveSubscriberLocalPriority ||
                    (this.DtoReceiveSubscriberLocalPriority != null &&
                    this.DtoReceiveSubscriberLocalPriority.Equals(input.DtoReceiveSubscriberLocalPriority))
                ) && 
                (
                    this.DtoReceiveSubscriberNetworkPriority == input.DtoReceiveSubscriberNetworkPriority ||
                    (this.DtoReceiveSubscriberNetworkPriority != null &&
                    this.DtoReceiveSubscriberNetworkPriority.Equals(input.DtoReceiveSubscriberNetworkPriority))
                ) && 
                (
                    this.DtoSendEnabled == input.DtoSendEnabled ||
                    (this.DtoSendEnabled != null &&
                    this.DtoSendEnabled.Equals(input.DtoSendEnabled))
                ) && 
                (
                    this.DynamicEndpointCreateDurableEnabled == input.DynamicEndpointCreateDurableEnabled ||
                    (this.DynamicEndpointCreateDurableEnabled != null &&
                    this.DynamicEndpointCreateDurableEnabled.Equals(input.DynamicEndpointCreateDurableEnabled))
                ) && 
                (
                    this.DynamicEndpointRespectTtlEnabled == input.DynamicEndpointRespectTtlEnabled ||
                    (this.DynamicEndpointRespectTtlEnabled != null &&
                    this.DynamicEndpointRespectTtlEnabled.Equals(input.DynamicEndpointRespectTtlEnabled))
                ) && 
                (
                    this.GuaranteedReceiveAckTimeout == input.GuaranteedReceiveAckTimeout ||
                    (this.GuaranteedReceiveAckTimeout != null &&
                    this.GuaranteedReceiveAckTimeout.Equals(input.GuaranteedReceiveAckTimeout))
                ) && 
                (
                    this.GuaranteedReceiveReconnectRetryCount == input.GuaranteedReceiveReconnectRetryCount ||
                    (this.GuaranteedReceiveReconnectRetryCount != null &&
                    this.GuaranteedReceiveReconnectRetryCount.Equals(input.GuaranteedReceiveReconnectRetryCount))
                ) && 
                (
                    this.GuaranteedReceiveReconnectRetryWait == input.GuaranteedReceiveReconnectRetryWait ||
                    (this.GuaranteedReceiveReconnectRetryWait != null &&
                    this.GuaranteedReceiveReconnectRetryWait.Equals(input.GuaranteedReceiveReconnectRetryWait))
                ) && 
                (
                    this.GuaranteedReceiveWindowSize == input.GuaranteedReceiveWindowSize ||
                    (this.GuaranteedReceiveWindowSize != null &&
                    this.GuaranteedReceiveWindowSize.Equals(input.GuaranteedReceiveWindowSize))
                ) && 
                (
                    this.GuaranteedReceiveWindowSizeAckThreshold == input.GuaranteedReceiveWindowSizeAckThreshold ||
                    (this.GuaranteedReceiveWindowSizeAckThreshold != null &&
                    this.GuaranteedReceiveWindowSizeAckThreshold.Equals(input.GuaranteedReceiveWindowSizeAckThreshold))
                ) && 
                (
                    this.GuaranteedSendAckTimeout == input.GuaranteedSendAckTimeout ||
                    (this.GuaranteedSendAckTimeout != null &&
                    this.GuaranteedSendAckTimeout.Equals(input.GuaranteedSendAckTimeout))
                ) && 
                (
                    this.GuaranteedSendWindowSize == input.GuaranteedSendWindowSize ||
                    (this.GuaranteedSendWindowSize != null &&
                    this.GuaranteedSendWindowSize.Equals(input.GuaranteedSendWindowSize))
                ) && 
                (
                    this.MessagingDefaultDeliveryMode == input.MessagingDefaultDeliveryMode ||
                    (this.MessagingDefaultDeliveryMode != null &&
                    this.MessagingDefaultDeliveryMode.Equals(input.MessagingDefaultDeliveryMode))
                ) && 
                (
                    this.MessagingDefaultDmqEligibleEnabled == input.MessagingDefaultDmqEligibleEnabled ||
                    (this.MessagingDefaultDmqEligibleEnabled != null &&
                    this.MessagingDefaultDmqEligibleEnabled.Equals(input.MessagingDefaultDmqEligibleEnabled))
                ) && 
                (
                    this.MessagingDefaultElidingEligibleEnabled == input.MessagingDefaultElidingEligibleEnabled ||
                    (this.MessagingDefaultElidingEligibleEnabled != null &&
                    this.MessagingDefaultElidingEligibleEnabled.Equals(input.MessagingDefaultElidingEligibleEnabled))
                ) && 
                (
                    this.MessagingJmsxUserIdEnabled == input.MessagingJmsxUserIdEnabled ||
                    (this.MessagingJmsxUserIdEnabled != null &&
                    this.MessagingJmsxUserIdEnabled.Equals(input.MessagingJmsxUserIdEnabled))
                ) && 
                (
                    this.MessagingTextInXmlPayloadEnabled == input.MessagingTextInXmlPayloadEnabled ||
                    (this.MessagingTextInXmlPayloadEnabled != null &&
                    this.MessagingTextInXmlPayloadEnabled.Equals(input.MessagingTextInXmlPayloadEnabled))
                ) && 
                (
                    this.MsgVpnName == input.MsgVpnName ||
                    (this.MsgVpnName != null &&
                    this.MsgVpnName.Equals(input.MsgVpnName))
                ) && 
                (
                    this.TransportCompressionLevel == input.TransportCompressionLevel ||
                    (this.TransportCompressionLevel != null &&
                    this.TransportCompressionLevel.Equals(input.TransportCompressionLevel))
                ) && 
                (
                    this.TransportConnectRetryCount == input.TransportConnectRetryCount ||
                    (this.TransportConnectRetryCount != null &&
                    this.TransportConnectRetryCount.Equals(input.TransportConnectRetryCount))
                ) && 
                (
                    this.TransportConnectRetryPerHostCount == input.TransportConnectRetryPerHostCount ||
                    (this.TransportConnectRetryPerHostCount != null &&
                    this.TransportConnectRetryPerHostCount.Equals(input.TransportConnectRetryPerHostCount))
                ) && 
                (
                    this.TransportConnectTimeout == input.TransportConnectTimeout ||
                    (this.TransportConnectTimeout != null &&
                    this.TransportConnectTimeout.Equals(input.TransportConnectTimeout))
                ) && 
                (
                    this.TransportDirectTransportEnabled == input.TransportDirectTransportEnabled ||
                    (this.TransportDirectTransportEnabled != null &&
                    this.TransportDirectTransportEnabled.Equals(input.TransportDirectTransportEnabled))
                ) && 
                (
                    this.TransportKeepaliveCount == input.TransportKeepaliveCount ||
                    (this.TransportKeepaliveCount != null &&
                    this.TransportKeepaliveCount.Equals(input.TransportKeepaliveCount))
                ) && 
                (
                    this.TransportKeepaliveEnabled == input.TransportKeepaliveEnabled ||
                    (this.TransportKeepaliveEnabled != null &&
                    this.TransportKeepaliveEnabled.Equals(input.TransportKeepaliveEnabled))
                ) && 
                (
                    this.TransportKeepaliveInterval == input.TransportKeepaliveInterval ||
                    (this.TransportKeepaliveInterval != null &&
                    this.TransportKeepaliveInterval.Equals(input.TransportKeepaliveInterval))
                ) && 
                (
                    this.TransportMsgCallbackOnIoThreadEnabled == input.TransportMsgCallbackOnIoThreadEnabled ||
                    (this.TransportMsgCallbackOnIoThreadEnabled != null &&
                    this.TransportMsgCallbackOnIoThreadEnabled.Equals(input.TransportMsgCallbackOnIoThreadEnabled))
                ) && 
                (
                    this.TransportOptimizeDirectEnabled == input.TransportOptimizeDirectEnabled ||
                    (this.TransportOptimizeDirectEnabled != null &&
                    this.TransportOptimizeDirectEnabled.Equals(input.TransportOptimizeDirectEnabled))
                ) && 
                (
                    this.TransportPort == input.TransportPort ||
                    (this.TransportPort != null &&
                    this.TransportPort.Equals(input.TransportPort))
                ) && 
                (
                    this.TransportReadTimeout == input.TransportReadTimeout ||
                    (this.TransportReadTimeout != null &&
                    this.TransportReadTimeout.Equals(input.TransportReadTimeout))
                ) && 
                (
                    this.TransportReceiveBufferSize == input.TransportReceiveBufferSize ||
                    (this.TransportReceiveBufferSize != null &&
                    this.TransportReceiveBufferSize.Equals(input.TransportReceiveBufferSize))
                ) && 
                (
                    this.TransportReconnectRetryCount == input.TransportReconnectRetryCount ||
                    (this.TransportReconnectRetryCount != null &&
                    this.TransportReconnectRetryCount.Equals(input.TransportReconnectRetryCount))
                ) && 
                (
                    this.TransportReconnectRetryWait == input.TransportReconnectRetryWait ||
                    (this.TransportReconnectRetryWait != null &&
                    this.TransportReconnectRetryWait.Equals(input.TransportReconnectRetryWait))
                ) && 
                (
                    this.TransportSendBufferSize == input.TransportSendBufferSize ||
                    (this.TransportSendBufferSize != null &&
                    this.TransportSendBufferSize.Equals(input.TransportSendBufferSize))
                ) && 
                (
                    this.TransportTcpNoDelayEnabled == input.TransportTcpNoDelayEnabled ||
                    (this.TransportTcpNoDelayEnabled != null &&
                    this.TransportTcpNoDelayEnabled.Equals(input.TransportTcpNoDelayEnabled))
                ) && 
                (
                    this.XaEnabled == input.XaEnabled ||
                    (this.XaEnabled != null &&
                    this.XaEnabled.Equals(input.XaEnabled))
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
                if (this.AllowDuplicateClientIdEnabled != null)
                    hashCode = hashCode * 59 + this.AllowDuplicateClientIdEnabled.GetHashCode();
                if (this.ClientDescription != null)
                    hashCode = hashCode * 59 + this.ClientDescription.GetHashCode();
                if (this.ClientId != null)
                    hashCode = hashCode * 59 + this.ClientId.GetHashCode();
                if (this.ConnectionFactoryName != null)
                    hashCode = hashCode * 59 + this.ConnectionFactoryName.GetHashCode();
                if (this.DtoReceiveOverrideEnabled != null)
                    hashCode = hashCode * 59 + this.DtoReceiveOverrideEnabled.GetHashCode();
                if (this.DtoReceiveSubscriberLocalPriority != null)
                    hashCode = hashCode * 59 + this.DtoReceiveSubscriberLocalPriority.GetHashCode();
                if (this.DtoReceiveSubscriberNetworkPriority != null)
                    hashCode = hashCode * 59 + this.DtoReceiveSubscriberNetworkPriority.GetHashCode();
                if (this.DtoSendEnabled != null)
                    hashCode = hashCode * 59 + this.DtoSendEnabled.GetHashCode();
                if (this.DynamicEndpointCreateDurableEnabled != null)
                    hashCode = hashCode * 59 + this.DynamicEndpointCreateDurableEnabled.GetHashCode();
                if (this.DynamicEndpointRespectTtlEnabled != null)
                    hashCode = hashCode * 59 + this.DynamicEndpointRespectTtlEnabled.GetHashCode();
                if (this.GuaranteedReceiveAckTimeout != null)
                    hashCode = hashCode * 59 + this.GuaranteedReceiveAckTimeout.GetHashCode();
                if (this.GuaranteedReceiveReconnectRetryCount != null)
                    hashCode = hashCode * 59 + this.GuaranteedReceiveReconnectRetryCount.GetHashCode();
                if (this.GuaranteedReceiveReconnectRetryWait != null)
                    hashCode = hashCode * 59 + this.GuaranteedReceiveReconnectRetryWait.GetHashCode();
                if (this.GuaranteedReceiveWindowSize != null)
                    hashCode = hashCode * 59 + this.GuaranteedReceiveWindowSize.GetHashCode();
                if (this.GuaranteedReceiveWindowSizeAckThreshold != null)
                    hashCode = hashCode * 59 + this.GuaranteedReceiveWindowSizeAckThreshold.GetHashCode();
                if (this.GuaranteedSendAckTimeout != null)
                    hashCode = hashCode * 59 + this.GuaranteedSendAckTimeout.GetHashCode();
                if (this.GuaranteedSendWindowSize != null)
                    hashCode = hashCode * 59 + this.GuaranteedSendWindowSize.GetHashCode();
                if (this.MessagingDefaultDeliveryMode != null)
                    hashCode = hashCode * 59 + this.MessagingDefaultDeliveryMode.GetHashCode();
                if (this.MessagingDefaultDmqEligibleEnabled != null)
                    hashCode = hashCode * 59 + this.MessagingDefaultDmqEligibleEnabled.GetHashCode();
                if (this.MessagingDefaultElidingEligibleEnabled != null)
                    hashCode = hashCode * 59 + this.MessagingDefaultElidingEligibleEnabled.GetHashCode();
                if (this.MessagingJmsxUserIdEnabled != null)
                    hashCode = hashCode * 59 + this.MessagingJmsxUserIdEnabled.GetHashCode();
                if (this.MessagingTextInXmlPayloadEnabled != null)
                    hashCode = hashCode * 59 + this.MessagingTextInXmlPayloadEnabled.GetHashCode();
                if (this.MsgVpnName != null)
                    hashCode = hashCode * 59 + this.MsgVpnName.GetHashCode();
                if (this.TransportCompressionLevel != null)
                    hashCode = hashCode * 59 + this.TransportCompressionLevel.GetHashCode();
                if (this.TransportConnectRetryCount != null)
                    hashCode = hashCode * 59 + this.TransportConnectRetryCount.GetHashCode();
                if (this.TransportConnectRetryPerHostCount != null)
                    hashCode = hashCode * 59 + this.TransportConnectRetryPerHostCount.GetHashCode();
                if (this.TransportConnectTimeout != null)
                    hashCode = hashCode * 59 + this.TransportConnectTimeout.GetHashCode();
                if (this.TransportDirectTransportEnabled != null)
                    hashCode = hashCode * 59 + this.TransportDirectTransportEnabled.GetHashCode();
                if (this.TransportKeepaliveCount != null)
                    hashCode = hashCode * 59 + this.TransportKeepaliveCount.GetHashCode();
                if (this.TransportKeepaliveEnabled != null)
                    hashCode = hashCode * 59 + this.TransportKeepaliveEnabled.GetHashCode();
                if (this.TransportKeepaliveInterval != null)
                    hashCode = hashCode * 59 + this.TransportKeepaliveInterval.GetHashCode();
                if (this.TransportMsgCallbackOnIoThreadEnabled != null)
                    hashCode = hashCode * 59 + this.TransportMsgCallbackOnIoThreadEnabled.GetHashCode();
                if (this.TransportOptimizeDirectEnabled != null)
                    hashCode = hashCode * 59 + this.TransportOptimizeDirectEnabled.GetHashCode();
                if (this.TransportPort != null)
                    hashCode = hashCode * 59 + this.TransportPort.GetHashCode();
                if (this.TransportReadTimeout != null)
                    hashCode = hashCode * 59 + this.TransportReadTimeout.GetHashCode();
                if (this.TransportReceiveBufferSize != null)
                    hashCode = hashCode * 59 + this.TransportReceiveBufferSize.GetHashCode();
                if (this.TransportReconnectRetryCount != null)
                    hashCode = hashCode * 59 + this.TransportReconnectRetryCount.GetHashCode();
                if (this.TransportReconnectRetryWait != null)
                    hashCode = hashCode * 59 + this.TransportReconnectRetryWait.GetHashCode();
                if (this.TransportSendBufferSize != null)
                    hashCode = hashCode * 59 + this.TransportSendBufferSize.GetHashCode();
                if (this.TransportTcpNoDelayEnabled != null)
                    hashCode = hashCode * 59 + this.TransportTcpNoDelayEnabled.GetHashCode();
                if (this.XaEnabled != null)
                    hashCode = hashCode * 59 + this.XaEnabled.GetHashCode();
                return hashCode;
            }
        }
    }
}
