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
    /// MsgVpnRestDeliveryPointRestConsumer
    /// </summary>
    [DataContract]
        public partial class MsgVpnRestDeliveryPointRestConsumer :  IEquatable<MsgVpnRestDeliveryPointRestConsumer>
    {
        /// <summary>
        /// The authentication scheme used by the REST Consumer to login to the REST host. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - Login with no authentication. This may be useful for anonymous connections or when a REST Consumer does not require authentication. \&quot;http-basic\&quot; - Login with a username and optional password according to HTTP Basic authentication as per RFC2616. \&quot;client-certificate\&quot; - Login with a client TLS certificate as per RFC5246. Client certificate authentication is only available on TLS connections. \&quot;http-header\&quot; - Login with a specified HTTP header. \&quot;oauth-client\&quot; - Login with OAuth 2.0 client credentials. \&quot;oauth-jwt\&quot; - Login with OAuth (RFC 7523 JWT Profile). \&quot;transparent\&quot; - Login using the Authorization header from the message properties, if present. Transparent authentication passes along existing Authorization header metadata instead of discarding it. Note that if the message is coming from a REST producer, the REST service must be configured to forward the Authorization header. \&quot;aws\&quot; - Login using AWS Signature Version 4 authentication (AWS4-HMAC-SHA256). &lt;/pre&gt; 
        /// </summary>
        /// <value>The authentication scheme used by the REST Consumer to login to the REST host. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - Login with no authentication. This may be useful for anonymous connections or when a REST Consumer does not require authentication. \&quot;http-basic\&quot; - Login with a username and optional password according to HTTP Basic authentication as per RFC2616. \&quot;client-certificate\&quot; - Login with a client TLS certificate as per RFC5246. Client certificate authentication is only available on TLS connections. \&quot;http-header\&quot; - Login with a specified HTTP header. \&quot;oauth-client\&quot; - Login with OAuth 2.0 client credentials. \&quot;oauth-jwt\&quot; - Login with OAuth (RFC 7523 JWT Profile). \&quot;transparent\&quot; - Login using the Authorization header from the message properties, if present. Transparent authentication passes along existing Authorization header metadata instead of discarding it. Note that if the message is coming from a REST producer, the REST service must be configured to forward the Authorization header. \&quot;aws\&quot; - Login using AWS Signature Version 4 authentication (AWS4-HMAC-SHA256). &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum AuthenticationSchemeEnum
        {
            /// <summary>
            /// Enum None for value: none
            /// </summary>
            [EnumMember(Value = "none")]
            None = 1,
            /// <summary>
            /// Enum HttpBasic for value: http-basic
            /// </summary>
            [EnumMember(Value = "http-basic")]
            HttpBasic = 2,
            /// <summary>
            /// Enum ClientCertificate for value: client-certificate
            /// </summary>
            [EnumMember(Value = "client-certificate")]
            ClientCertificate = 3,
            /// <summary>
            /// Enum HttpHeader for value: http-header
            /// </summary>
            [EnumMember(Value = "http-header")]
            HttpHeader = 4,
            /// <summary>
            /// Enum OauthClient for value: oauth-client
            /// </summary>
            [EnumMember(Value = "oauth-client")]
            OauthClient = 5,
            /// <summary>
            /// Enum OauthJwt for value: oauth-jwt
            /// </summary>
            [EnumMember(Value = "oauth-jwt")]
            OauthJwt = 6,
            /// <summary>
            /// Enum Transparent for value: transparent
            /// </summary>
            [EnumMember(Value = "transparent")]
            Transparent = 7,
            /// <summary>
            /// Enum Aws for value: aws
            /// </summary>
            [EnumMember(Value = "aws")]
            Aws = 8        }
        /// <summary>
        /// The authentication scheme used by the REST Consumer to login to the REST host. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - Login with no authentication. This may be useful for anonymous connections or when a REST Consumer does not require authentication. \&quot;http-basic\&quot; - Login with a username and optional password according to HTTP Basic authentication as per RFC2616. \&quot;client-certificate\&quot; - Login with a client TLS certificate as per RFC5246. Client certificate authentication is only available on TLS connections. \&quot;http-header\&quot; - Login with a specified HTTP header. \&quot;oauth-client\&quot; - Login with OAuth 2.0 client credentials. \&quot;oauth-jwt\&quot; - Login with OAuth (RFC 7523 JWT Profile). \&quot;transparent\&quot; - Login using the Authorization header from the message properties, if present. Transparent authentication passes along existing Authorization header metadata instead of discarding it. Note that if the message is coming from a REST producer, the REST service must be configured to forward the Authorization header. \&quot;aws\&quot; - Login using AWS Signature Version 4 authentication (AWS4-HMAC-SHA256). &lt;/pre&gt; 
        /// </summary>
        /// <value>The authentication scheme used by the REST Consumer to login to the REST host. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - Login with no authentication. This may be useful for anonymous connections or when a REST Consumer does not require authentication. \&quot;http-basic\&quot; - Login with a username and optional password according to HTTP Basic authentication as per RFC2616. \&quot;client-certificate\&quot; - Login with a client TLS certificate as per RFC5246. Client certificate authentication is only available on TLS connections. \&quot;http-header\&quot; - Login with a specified HTTP header. \&quot;oauth-client\&quot; - Login with OAuth 2.0 client credentials. \&quot;oauth-jwt\&quot; - Login with OAuth (RFC 7523 JWT Profile). \&quot;transparent\&quot; - Login using the Authorization header from the message properties, if present. Transparent authentication passes along existing Authorization header metadata instead of discarding it. Note that if the message is coming from a REST producer, the REST service must be configured to forward the Authorization header. \&quot;aws\&quot; - Login using AWS Signature Version 4 authentication (AWS4-HMAC-SHA256). &lt;/pre&gt; </value>
        [DataMember(Name="authenticationScheme", EmitDefaultValue=false)]
        public AuthenticationSchemeEnum? AuthenticationScheme { get; set; }
        /// <summary>
        /// The HTTP method to use (POST or PUT). This is used only when operating in the REST service \&quot;messaging\&quot; mode and is ignored in \&quot;gateway\&quot; mode. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;post\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;post\&quot; - Use the POST HTTP method. \&quot;put\&quot; - Use the PUT HTTP method. &lt;/pre&gt;  Available since 2.17.
        /// </summary>
        /// <value>The HTTP method to use (POST or PUT). This is used only when operating in the REST service \&quot;messaging\&quot; mode and is ignored in \&quot;gateway\&quot; mode. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;post\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;post\&quot; - Use the POST HTTP method. \&quot;put\&quot; - Use the PUT HTTP method. &lt;/pre&gt;  Available since 2.17.</value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum HttpMethodEnum
        {
            /// <summary>
            /// Enum Post for value: post
            /// </summary>
            [EnumMember(Value = "post")]
            Post = 1,
            /// <summary>
            /// Enum Put for value: put
            /// </summary>
            [EnumMember(Value = "put")]
            Put = 2        }
        /// <summary>
        /// The HTTP method to use (POST or PUT). This is used only when operating in the REST service \&quot;messaging\&quot; mode and is ignored in \&quot;gateway\&quot; mode. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;post\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;post\&quot; - Use the POST HTTP method. \&quot;put\&quot; - Use the PUT HTTP method. &lt;/pre&gt;  Available since 2.17.
        /// </summary>
        /// <value>The HTTP method to use (POST or PUT). This is used only when operating in the REST service \&quot;messaging\&quot; mode and is ignored in \&quot;gateway\&quot; mode. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;post\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;post\&quot; - Use the POST HTTP method. \&quot;put\&quot; - Use the PUT HTTP method. &lt;/pre&gt;  Available since 2.17.</value>
        [DataMember(Name="httpMethod", EmitDefaultValue=false)]
        public HttpMethodEnum? HttpMethod { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="MsgVpnRestDeliveryPointRestConsumer" /> class.
        /// </summary>
        /// <param name="authenticationAwsAccessKeyId">The AWS access key id. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.26..</param>
        /// <param name="authenticationAwsRegion">The AWS region id. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.26..</param>
        /// <param name="authenticationAwsSecretAccessKey">The AWS secret access key. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.26..</param>
        /// <param name="authenticationAwsService">The AWS service id. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.26..</param>
        /// <param name="authenticationClientCertContent">The PEM formatted content for the client certificate that the REST Consumer will present to the REST host. It must consist of a private key and between one and three certificates comprising the certificate trust chain. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.9..</param>
        /// <param name="authenticationClientCertPassword">The password for the client certificate. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.9..</param>
        /// <param name="authenticationHttpBasicPassword">The password for the username. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="authenticationHttpBasicUsername">The username that the REST Consumer will use to login to the REST host. Normally a username is only configured when basic authentication is selected for the REST Consumer. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="authenticationHttpHeaderName">The authentication header name. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.15..</param>
        /// <param name="authenticationHttpHeaderValue">The authentication header value. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.15..</param>
        /// <param name="authenticationOauthClientId">The OAuth client ID. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.19..</param>
        /// <param name="authenticationOauthClientScope">The OAuth scope. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.19..</param>
        /// <param name="authenticationOauthClientSecret">The OAuth client secret. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.19..</param>
        /// <param name="authenticationOauthClientTokenEndpoint">The OAuth token endpoint URL that the REST Consumer will use to request a token for login to the REST host. Must begin with \&quot;https\&quot;. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.19..</param>
        /// <param name="authenticationOauthClientTokenExpiryDefault">The default expiry time for a token, in seconds. Only used when the token endpoint does not return an expiry time. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;900&#x60;. Available since 2.30..</param>
        /// <param name="authenticationOauthJwtSecretKey">The OAuth secret key used to sign the token request JWT. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.21..</param>
        /// <param name="authenticationOauthJwtTokenEndpoint">The OAuth token endpoint URL that the REST Consumer will use to request a token for login to the REST host. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.21..</param>
        /// <param name="authenticationOauthJwtTokenExpiryDefault">The default expiry time for a token, in seconds. Only used when the token endpoint does not return an expiry time. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;900&#x60;. Available since 2.30..</param>
        /// <param name="authenticationScheme">The authentication scheme used by the REST Consumer to login to the REST host. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - Login with no authentication. This may be useful for anonymous connections or when a REST Consumer does not require authentication. \&quot;http-basic\&quot; - Login with a username and optional password according to HTTP Basic authentication as per RFC2616. \&quot;client-certificate\&quot; - Login with a client TLS certificate as per RFC5246. Client certificate authentication is only available on TLS connections. \&quot;http-header\&quot; - Login with a specified HTTP header. \&quot;oauth-client\&quot; - Login with OAuth 2.0 client credentials. \&quot;oauth-jwt\&quot; - Login with OAuth (RFC 7523 JWT Profile). \&quot;transparent\&quot; - Login using the Authorization header from the message properties, if present. Transparent authentication passes along existing Authorization header metadata instead of discarding it. Note that if the message is coming from a REST producer, the REST service must be configured to forward the Authorization header. \&quot;aws\&quot; - Login using AWS Signature Version 4 authentication (AWS4-HMAC-SHA256). &lt;/pre&gt; .</param>
        /// <param name="enabled">Enable or disable the REST Consumer. When disabled, no connections are initiated or messages delivered to this particular REST Consumer. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="httpMethod">The HTTP method to use (POST or PUT). This is used only when operating in the REST service \&quot;messaging\&quot; mode and is ignored in \&quot;gateway\&quot; mode. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;post\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;post\&quot; - Use the POST HTTP method. \&quot;put\&quot; - Use the PUT HTTP method. &lt;/pre&gt;  Available since 2.17..</param>
        /// <param name="localInterface">The interface that will be used for all outgoing connections associated with the REST Consumer. When unspecified, an interface is automatically chosen. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="maxPostWaitTime">The maximum amount of time (in seconds) to wait for an HTTP POST response from the REST Consumer. Once this time is exceeded, the TCP connection is reset. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;30&#x60;..</param>
        /// <param name="msgVpnName">The name of the Message VPN..</param>
        /// <param name="outgoingConnectionCount">The number of concurrent TCP connections open to the REST Consumer. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;..</param>
        /// <param name="remoteHost">The IP address or DNS name to which the broker is to connect to deliver messages for the REST Consumer. A host value must be configured for the REST Consumer to be operationally up. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="remotePort">The port associated with the host of the REST Consumer. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;8080&#x60;..</param>
        /// <param name="restConsumerName">The name of the REST Consumer..</param>
        /// <param name="restDeliveryPointName">The name of the REST Delivery Point..</param>
        /// <param name="retryDelay">The number of seconds that must pass before retrying the remote REST Consumer connection. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;..</param>
        /// <param name="tlsCipherSuiteList">The colon-separated list of cipher suites the REST Consumer uses in its encrypted connection. The value &#x60;\&quot;default\&quot;&#x60; implies all supported suites ordered from most secure to least secure. The list of default cipher suites is available in the &#x60;tlsCipherSuiteMsgBackboneDefaultList&#x60; attribute of the Broker object in the Monitoring API. The REST Consumer should choose the first suite from this list that it supports. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;..</param>
        /// <param name="tlsEnabled">Enable or disable encryption (TLS) for the REST Consumer. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        public MsgVpnRestDeliveryPointRestConsumer(string authenticationAwsAccessKeyId = default(string), string authenticationAwsRegion = default(string), string authenticationAwsSecretAccessKey = default(string), string authenticationAwsService = default(string), string authenticationClientCertContent = default(string), string authenticationClientCertPassword = default(string), string authenticationHttpBasicPassword = default(string), string authenticationHttpBasicUsername = default(string), string authenticationHttpHeaderName = default(string), string authenticationHttpHeaderValue = default(string), string authenticationOauthClientId = default(string), string authenticationOauthClientScope = default(string), string authenticationOauthClientSecret = default(string), string authenticationOauthClientTokenEndpoint = default(string), int? authenticationOauthClientTokenExpiryDefault = default(int?), string authenticationOauthJwtSecretKey = default(string), string authenticationOauthJwtTokenEndpoint = default(string), int? authenticationOauthJwtTokenExpiryDefault = default(int?), AuthenticationSchemeEnum? authenticationScheme = default(AuthenticationSchemeEnum?), bool? enabled = default(bool?), HttpMethodEnum? httpMethod = default(HttpMethodEnum?), string localInterface = default(string), int? maxPostWaitTime = default(int?), string msgVpnName = default(string), int? outgoingConnectionCount = default(int?), string remoteHost = default(string), long? remotePort = default(long?), string restConsumerName = default(string), string restDeliveryPointName = default(string), int? retryDelay = default(int?), string tlsCipherSuiteList = default(string), bool? tlsEnabled = default(bool?))
        {
            this.AuthenticationAwsAccessKeyId = authenticationAwsAccessKeyId;
            this.AuthenticationAwsRegion = authenticationAwsRegion;
            this.AuthenticationAwsSecretAccessKey = authenticationAwsSecretAccessKey;
            this.AuthenticationAwsService = authenticationAwsService;
            this.AuthenticationClientCertContent = authenticationClientCertContent;
            this.AuthenticationClientCertPassword = authenticationClientCertPassword;
            this.AuthenticationHttpBasicPassword = authenticationHttpBasicPassword;
            this.AuthenticationHttpBasicUsername = authenticationHttpBasicUsername;
            this.AuthenticationHttpHeaderName = authenticationHttpHeaderName;
            this.AuthenticationHttpHeaderValue = authenticationHttpHeaderValue;
            this.AuthenticationOauthClientId = authenticationOauthClientId;
            this.AuthenticationOauthClientScope = authenticationOauthClientScope;
            this.AuthenticationOauthClientSecret = authenticationOauthClientSecret;
            this.AuthenticationOauthClientTokenEndpoint = authenticationOauthClientTokenEndpoint;
            this.AuthenticationOauthClientTokenExpiryDefault = authenticationOauthClientTokenExpiryDefault;
            this.AuthenticationOauthJwtSecretKey = authenticationOauthJwtSecretKey;
            this.AuthenticationOauthJwtTokenEndpoint = authenticationOauthJwtTokenEndpoint;
            this.AuthenticationOauthJwtTokenExpiryDefault = authenticationOauthJwtTokenExpiryDefault;
            this.AuthenticationScheme = authenticationScheme;
            this.Enabled = enabled;
            this.HttpMethod = httpMethod;
            this.LocalInterface = localInterface;
            this.MaxPostWaitTime = maxPostWaitTime;
            this.MsgVpnName = msgVpnName;
            this.OutgoingConnectionCount = outgoingConnectionCount;
            this.RemoteHost = remoteHost;
            this.RemotePort = remotePort;
            this.RestConsumerName = restConsumerName;
            this.RestDeliveryPointName = restDeliveryPointName;
            this.RetryDelay = retryDelay;
            this.TlsCipherSuiteList = tlsCipherSuiteList;
            this.TlsEnabled = tlsEnabled;
        }
        
        /// <summary>
        /// The AWS access key id. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.26.
        /// </summary>
        /// <value>The AWS access key id. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.26.</value>
        [DataMember(Name="authenticationAwsAccessKeyId", EmitDefaultValue=false)]
        public string AuthenticationAwsAccessKeyId { get; set; }

        /// <summary>
        /// The AWS region id. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.26.
        /// </summary>
        /// <value>The AWS region id. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.26.</value>
        [DataMember(Name="authenticationAwsRegion", EmitDefaultValue=false)]
        public string AuthenticationAwsRegion { get; set; }

        /// <summary>
        /// The AWS secret access key. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.26.
        /// </summary>
        /// <value>The AWS secret access key. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.26.</value>
        [DataMember(Name="authenticationAwsSecretAccessKey", EmitDefaultValue=false)]
        public string AuthenticationAwsSecretAccessKey { get; set; }

        /// <summary>
        /// The AWS service id. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.26.
        /// </summary>
        /// <value>The AWS service id. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.26.</value>
        [DataMember(Name="authenticationAwsService", EmitDefaultValue=false)]
        public string AuthenticationAwsService { get; set; }

        /// <summary>
        /// The PEM formatted content for the client certificate that the REST Consumer will present to the REST host. It must consist of a private key and between one and three certificates comprising the certificate trust chain. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.9.
        /// </summary>
        /// <value>The PEM formatted content for the client certificate that the REST Consumer will present to the REST host. It must consist of a private key and between one and three certificates comprising the certificate trust chain. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.9.</value>
        [DataMember(Name="authenticationClientCertContent", EmitDefaultValue=false)]
        public string AuthenticationClientCertContent { get; set; }

        /// <summary>
        /// The password for the client certificate. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.9.
        /// </summary>
        /// <value>The password for the client certificate. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.9.</value>
        [DataMember(Name="authenticationClientCertPassword", EmitDefaultValue=false)]
        public string AuthenticationClientCertPassword { get; set; }

        /// <summary>
        /// The password for the username. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The password for the username. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="authenticationHttpBasicPassword", EmitDefaultValue=false)]
        public string AuthenticationHttpBasicPassword { get; set; }

        /// <summary>
        /// The username that the REST Consumer will use to login to the REST host. Normally a username is only configured when basic authentication is selected for the REST Consumer. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The username that the REST Consumer will use to login to the REST host. Normally a username is only configured when basic authentication is selected for the REST Consumer. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="authenticationHttpBasicUsername", EmitDefaultValue=false)]
        public string AuthenticationHttpBasicUsername { get; set; }

        /// <summary>
        /// The authentication header name. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.15.
        /// </summary>
        /// <value>The authentication header name. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.15.</value>
        [DataMember(Name="authenticationHttpHeaderName", EmitDefaultValue=false)]
        public string AuthenticationHttpHeaderName { get; set; }

        /// <summary>
        /// The authentication header value. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.15.
        /// </summary>
        /// <value>The authentication header value. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.15.</value>
        [DataMember(Name="authenticationHttpHeaderValue", EmitDefaultValue=false)]
        public string AuthenticationHttpHeaderValue { get; set; }

        /// <summary>
        /// The OAuth client ID. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.19.
        /// </summary>
        /// <value>The OAuth client ID. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.19.</value>
        [DataMember(Name="authenticationOauthClientId", EmitDefaultValue=false)]
        public string AuthenticationOauthClientId { get; set; }

        /// <summary>
        /// The OAuth scope. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.19.
        /// </summary>
        /// <value>The OAuth scope. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.19.</value>
        [DataMember(Name="authenticationOauthClientScope", EmitDefaultValue=false)]
        public string AuthenticationOauthClientScope { get; set; }

        /// <summary>
        /// The OAuth client secret. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.19.
        /// </summary>
        /// <value>The OAuth client secret. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.19.</value>
        [DataMember(Name="authenticationOauthClientSecret", EmitDefaultValue=false)]
        public string AuthenticationOauthClientSecret { get; set; }

        /// <summary>
        /// The OAuth token endpoint URL that the REST Consumer will use to request a token for login to the REST host. Must begin with \&quot;https\&quot;. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.19.
        /// </summary>
        /// <value>The OAuth token endpoint URL that the REST Consumer will use to request a token for login to the REST host. Must begin with \&quot;https\&quot;. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.19.</value>
        [DataMember(Name="authenticationOauthClientTokenEndpoint", EmitDefaultValue=false)]
        public string AuthenticationOauthClientTokenEndpoint { get; set; }

        /// <summary>
        /// The default expiry time for a token, in seconds. Only used when the token endpoint does not return an expiry time. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;900&#x60;. Available since 2.30.
        /// </summary>
        /// <value>The default expiry time for a token, in seconds. Only used when the token endpoint does not return an expiry time. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;900&#x60;. Available since 2.30.</value>
        [DataMember(Name="authenticationOauthClientTokenExpiryDefault", EmitDefaultValue=false)]
        public int? AuthenticationOauthClientTokenExpiryDefault { get; set; }

        /// <summary>
        /// The OAuth secret key used to sign the token request JWT. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.21.
        /// </summary>
        /// <value>The OAuth secret key used to sign the token request JWT. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.21.</value>
        [DataMember(Name="authenticationOauthJwtSecretKey", EmitDefaultValue=false)]
        public string AuthenticationOauthJwtSecretKey { get; set; }

        /// <summary>
        /// The OAuth token endpoint URL that the REST Consumer will use to request a token for login to the REST host. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.21.
        /// </summary>
        /// <value>The OAuth token endpoint URL that the REST Consumer will use to request a token for login to the REST host. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.21.</value>
        [DataMember(Name="authenticationOauthJwtTokenEndpoint", EmitDefaultValue=false)]
        public string AuthenticationOauthJwtTokenEndpoint { get; set; }

        /// <summary>
        /// The default expiry time for a token, in seconds. Only used when the token endpoint does not return an expiry time. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;900&#x60;. Available since 2.30.
        /// </summary>
        /// <value>The default expiry time for a token, in seconds. Only used when the token endpoint does not return an expiry time. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;900&#x60;. Available since 2.30.</value>
        [DataMember(Name="authenticationOauthJwtTokenExpiryDefault", EmitDefaultValue=false)]
        public int? AuthenticationOauthJwtTokenExpiryDefault { get; set; }


        /// <summary>
        /// Enable or disable the REST Consumer. When disabled, no connections are initiated or messages delivered to this particular REST Consumer. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable the REST Consumer. When disabled, no connections are initiated or messages delivered to this particular REST Consumer. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="enabled", EmitDefaultValue=false)]
        public bool? Enabled { get; set; }


        /// <summary>
        /// The interface that will be used for all outgoing connections associated with the REST Consumer. When unspecified, an interface is automatically chosen. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The interface that will be used for all outgoing connections associated with the REST Consumer. When unspecified, an interface is automatically chosen. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="localInterface", EmitDefaultValue=false)]
        public string LocalInterface { get; set; }

        /// <summary>
        /// The maximum amount of time (in seconds) to wait for an HTTP POST response from the REST Consumer. Once this time is exceeded, the TCP connection is reset. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;30&#x60;.
        /// </summary>
        /// <value>The maximum amount of time (in seconds) to wait for an HTTP POST response from the REST Consumer. Once this time is exceeded, the TCP connection is reset. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;30&#x60;.</value>
        [DataMember(Name="maxPostWaitTime", EmitDefaultValue=false)]
        public int? MaxPostWaitTime { get; set; }

        /// <summary>
        /// The name of the Message VPN.
        /// </summary>
        /// <value>The name of the Message VPN.</value>
        [DataMember(Name="msgVpnName", EmitDefaultValue=false)]
        public string MsgVpnName { get; set; }

        /// <summary>
        /// The number of concurrent TCP connections open to the REST Consumer. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;.
        /// </summary>
        /// <value>The number of concurrent TCP connections open to the REST Consumer. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;.</value>
        [DataMember(Name="outgoingConnectionCount", EmitDefaultValue=false)]
        public int? OutgoingConnectionCount { get; set; }

        /// <summary>
        /// The IP address or DNS name to which the broker is to connect to deliver messages for the REST Consumer. A host value must be configured for the REST Consumer to be operationally up. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The IP address or DNS name to which the broker is to connect to deliver messages for the REST Consumer. A host value must be configured for the REST Consumer to be operationally up. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="remoteHost", EmitDefaultValue=false)]
        public string RemoteHost { get; set; }

        /// <summary>
        /// The port associated with the host of the REST Consumer. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;8080&#x60;.
        /// </summary>
        /// <value>The port associated with the host of the REST Consumer. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;8080&#x60;.</value>
        [DataMember(Name="remotePort", EmitDefaultValue=false)]
        public long? RemotePort { get; set; }

        /// <summary>
        /// The name of the REST Consumer.
        /// </summary>
        /// <value>The name of the REST Consumer.</value>
        [DataMember(Name="restConsumerName", EmitDefaultValue=false)]
        public string RestConsumerName { get; set; }

        /// <summary>
        /// The name of the REST Delivery Point.
        /// </summary>
        /// <value>The name of the REST Delivery Point.</value>
        [DataMember(Name="restDeliveryPointName", EmitDefaultValue=false)]
        public string RestDeliveryPointName { get; set; }

        /// <summary>
        /// The number of seconds that must pass before retrying the remote REST Consumer connection. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;.
        /// </summary>
        /// <value>The number of seconds that must pass before retrying the remote REST Consumer connection. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;.</value>
        [DataMember(Name="retryDelay", EmitDefaultValue=false)]
        public int? RetryDelay { get; set; }

        /// <summary>
        /// The colon-separated list of cipher suites the REST Consumer uses in its encrypted connection. The value &#x60;\&quot;default\&quot;&#x60; implies all supported suites ordered from most secure to least secure. The list of default cipher suites is available in the &#x60;tlsCipherSuiteMsgBackboneDefaultList&#x60; attribute of the Broker object in the Monitoring API. The REST Consumer should choose the first suite from this list that it supports. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;.
        /// </summary>
        /// <value>The colon-separated list of cipher suites the REST Consumer uses in its encrypted connection. The value &#x60;\&quot;default\&quot;&#x60; implies all supported suites ordered from most secure to least secure. The list of default cipher suites is available in the &#x60;tlsCipherSuiteMsgBackboneDefaultList&#x60; attribute of the Broker object in the Monitoring API. The REST Consumer should choose the first suite from this list that it supports. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;.</value>
        [DataMember(Name="tlsCipherSuiteList", EmitDefaultValue=false)]
        public string TlsCipherSuiteList { get; set; }

        /// <summary>
        /// Enable or disable encryption (TLS) for the REST Consumer. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable encryption (TLS) for the REST Consumer. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="tlsEnabled", EmitDefaultValue=false)]
        public bool? TlsEnabled { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class MsgVpnRestDeliveryPointRestConsumer {\n");
            sb.Append("  AuthenticationAwsAccessKeyId: ").Append(AuthenticationAwsAccessKeyId).Append("\n");
            sb.Append("  AuthenticationAwsRegion: ").Append(AuthenticationAwsRegion).Append("\n");
            sb.Append("  AuthenticationAwsSecretAccessKey: ").Append(AuthenticationAwsSecretAccessKey).Append("\n");
            sb.Append("  AuthenticationAwsService: ").Append(AuthenticationAwsService).Append("\n");
            sb.Append("  AuthenticationClientCertContent: ").Append(AuthenticationClientCertContent).Append("\n");
            sb.Append("  AuthenticationClientCertPassword: ").Append(AuthenticationClientCertPassword).Append("\n");
            sb.Append("  AuthenticationHttpBasicPassword: ").Append(AuthenticationHttpBasicPassword).Append("\n");
            sb.Append("  AuthenticationHttpBasicUsername: ").Append(AuthenticationHttpBasicUsername).Append("\n");
            sb.Append("  AuthenticationHttpHeaderName: ").Append(AuthenticationHttpHeaderName).Append("\n");
            sb.Append("  AuthenticationHttpHeaderValue: ").Append(AuthenticationHttpHeaderValue).Append("\n");
            sb.Append("  AuthenticationOauthClientId: ").Append(AuthenticationOauthClientId).Append("\n");
            sb.Append("  AuthenticationOauthClientScope: ").Append(AuthenticationOauthClientScope).Append("\n");
            sb.Append("  AuthenticationOauthClientSecret: ").Append(AuthenticationOauthClientSecret).Append("\n");
            sb.Append("  AuthenticationOauthClientTokenEndpoint: ").Append(AuthenticationOauthClientTokenEndpoint).Append("\n");
            sb.Append("  AuthenticationOauthClientTokenExpiryDefault: ").Append(AuthenticationOauthClientTokenExpiryDefault).Append("\n");
            sb.Append("  AuthenticationOauthJwtSecretKey: ").Append(AuthenticationOauthJwtSecretKey).Append("\n");
            sb.Append("  AuthenticationOauthJwtTokenEndpoint: ").Append(AuthenticationOauthJwtTokenEndpoint).Append("\n");
            sb.Append("  AuthenticationOauthJwtTokenExpiryDefault: ").Append(AuthenticationOauthJwtTokenExpiryDefault).Append("\n");
            sb.Append("  AuthenticationScheme: ").Append(AuthenticationScheme).Append("\n");
            sb.Append("  Enabled: ").Append(Enabled).Append("\n");
            sb.Append("  HttpMethod: ").Append(HttpMethod).Append("\n");
            sb.Append("  LocalInterface: ").Append(LocalInterface).Append("\n");
            sb.Append("  MaxPostWaitTime: ").Append(MaxPostWaitTime).Append("\n");
            sb.Append("  MsgVpnName: ").Append(MsgVpnName).Append("\n");
            sb.Append("  OutgoingConnectionCount: ").Append(OutgoingConnectionCount).Append("\n");
            sb.Append("  RemoteHost: ").Append(RemoteHost).Append("\n");
            sb.Append("  RemotePort: ").Append(RemotePort).Append("\n");
            sb.Append("  RestConsumerName: ").Append(RestConsumerName).Append("\n");
            sb.Append("  RestDeliveryPointName: ").Append(RestDeliveryPointName).Append("\n");
            sb.Append("  RetryDelay: ").Append(RetryDelay).Append("\n");
            sb.Append("  TlsCipherSuiteList: ").Append(TlsCipherSuiteList).Append("\n");
            sb.Append("  TlsEnabled: ").Append(TlsEnabled).Append("\n");
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
            return this.Equals(input as MsgVpnRestDeliveryPointRestConsumer);
        }

        /// <summary>
        /// Returns true if MsgVpnRestDeliveryPointRestConsumer instances are equal
        /// </summary>
        /// <param name="input">Instance of MsgVpnRestDeliveryPointRestConsumer to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(MsgVpnRestDeliveryPointRestConsumer input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.AuthenticationAwsAccessKeyId == input.AuthenticationAwsAccessKeyId ||
                    (this.AuthenticationAwsAccessKeyId != null &&
                    this.AuthenticationAwsAccessKeyId.Equals(input.AuthenticationAwsAccessKeyId))
                ) && 
                (
                    this.AuthenticationAwsRegion == input.AuthenticationAwsRegion ||
                    (this.AuthenticationAwsRegion != null &&
                    this.AuthenticationAwsRegion.Equals(input.AuthenticationAwsRegion))
                ) && 
                (
                    this.AuthenticationAwsSecretAccessKey == input.AuthenticationAwsSecretAccessKey ||
                    (this.AuthenticationAwsSecretAccessKey != null &&
                    this.AuthenticationAwsSecretAccessKey.Equals(input.AuthenticationAwsSecretAccessKey))
                ) && 
                (
                    this.AuthenticationAwsService == input.AuthenticationAwsService ||
                    (this.AuthenticationAwsService != null &&
                    this.AuthenticationAwsService.Equals(input.AuthenticationAwsService))
                ) && 
                (
                    this.AuthenticationClientCertContent == input.AuthenticationClientCertContent ||
                    (this.AuthenticationClientCertContent != null &&
                    this.AuthenticationClientCertContent.Equals(input.AuthenticationClientCertContent))
                ) && 
                (
                    this.AuthenticationClientCertPassword == input.AuthenticationClientCertPassword ||
                    (this.AuthenticationClientCertPassword != null &&
                    this.AuthenticationClientCertPassword.Equals(input.AuthenticationClientCertPassword))
                ) && 
                (
                    this.AuthenticationHttpBasicPassword == input.AuthenticationHttpBasicPassword ||
                    (this.AuthenticationHttpBasicPassword != null &&
                    this.AuthenticationHttpBasicPassword.Equals(input.AuthenticationHttpBasicPassword))
                ) && 
                (
                    this.AuthenticationHttpBasicUsername == input.AuthenticationHttpBasicUsername ||
                    (this.AuthenticationHttpBasicUsername != null &&
                    this.AuthenticationHttpBasicUsername.Equals(input.AuthenticationHttpBasicUsername))
                ) && 
                (
                    this.AuthenticationHttpHeaderName == input.AuthenticationHttpHeaderName ||
                    (this.AuthenticationHttpHeaderName != null &&
                    this.AuthenticationHttpHeaderName.Equals(input.AuthenticationHttpHeaderName))
                ) && 
                (
                    this.AuthenticationHttpHeaderValue == input.AuthenticationHttpHeaderValue ||
                    (this.AuthenticationHttpHeaderValue != null &&
                    this.AuthenticationHttpHeaderValue.Equals(input.AuthenticationHttpHeaderValue))
                ) && 
                (
                    this.AuthenticationOauthClientId == input.AuthenticationOauthClientId ||
                    (this.AuthenticationOauthClientId != null &&
                    this.AuthenticationOauthClientId.Equals(input.AuthenticationOauthClientId))
                ) && 
                (
                    this.AuthenticationOauthClientScope == input.AuthenticationOauthClientScope ||
                    (this.AuthenticationOauthClientScope != null &&
                    this.AuthenticationOauthClientScope.Equals(input.AuthenticationOauthClientScope))
                ) && 
                (
                    this.AuthenticationOauthClientSecret == input.AuthenticationOauthClientSecret ||
                    (this.AuthenticationOauthClientSecret != null &&
                    this.AuthenticationOauthClientSecret.Equals(input.AuthenticationOauthClientSecret))
                ) && 
                (
                    this.AuthenticationOauthClientTokenEndpoint == input.AuthenticationOauthClientTokenEndpoint ||
                    (this.AuthenticationOauthClientTokenEndpoint != null &&
                    this.AuthenticationOauthClientTokenEndpoint.Equals(input.AuthenticationOauthClientTokenEndpoint))
                ) && 
                (
                    this.AuthenticationOauthClientTokenExpiryDefault == input.AuthenticationOauthClientTokenExpiryDefault ||
                    (this.AuthenticationOauthClientTokenExpiryDefault != null &&
                    this.AuthenticationOauthClientTokenExpiryDefault.Equals(input.AuthenticationOauthClientTokenExpiryDefault))
                ) && 
                (
                    this.AuthenticationOauthJwtSecretKey == input.AuthenticationOauthJwtSecretKey ||
                    (this.AuthenticationOauthJwtSecretKey != null &&
                    this.AuthenticationOauthJwtSecretKey.Equals(input.AuthenticationOauthJwtSecretKey))
                ) && 
                (
                    this.AuthenticationOauthJwtTokenEndpoint == input.AuthenticationOauthJwtTokenEndpoint ||
                    (this.AuthenticationOauthJwtTokenEndpoint != null &&
                    this.AuthenticationOauthJwtTokenEndpoint.Equals(input.AuthenticationOauthJwtTokenEndpoint))
                ) && 
                (
                    this.AuthenticationOauthJwtTokenExpiryDefault == input.AuthenticationOauthJwtTokenExpiryDefault ||
                    (this.AuthenticationOauthJwtTokenExpiryDefault != null &&
                    this.AuthenticationOauthJwtTokenExpiryDefault.Equals(input.AuthenticationOauthJwtTokenExpiryDefault))
                ) && 
                (
                    this.AuthenticationScheme == input.AuthenticationScheme ||
                    (this.AuthenticationScheme != null &&
                    this.AuthenticationScheme.Equals(input.AuthenticationScheme))
                ) && 
                (
                    this.Enabled == input.Enabled ||
                    (this.Enabled != null &&
                    this.Enabled.Equals(input.Enabled))
                ) && 
                (
                    this.HttpMethod == input.HttpMethod ||
                    (this.HttpMethod != null &&
                    this.HttpMethod.Equals(input.HttpMethod))
                ) && 
                (
                    this.LocalInterface == input.LocalInterface ||
                    (this.LocalInterface != null &&
                    this.LocalInterface.Equals(input.LocalInterface))
                ) && 
                (
                    this.MaxPostWaitTime == input.MaxPostWaitTime ||
                    (this.MaxPostWaitTime != null &&
                    this.MaxPostWaitTime.Equals(input.MaxPostWaitTime))
                ) && 
                (
                    this.MsgVpnName == input.MsgVpnName ||
                    (this.MsgVpnName != null &&
                    this.MsgVpnName.Equals(input.MsgVpnName))
                ) && 
                (
                    this.OutgoingConnectionCount == input.OutgoingConnectionCount ||
                    (this.OutgoingConnectionCount != null &&
                    this.OutgoingConnectionCount.Equals(input.OutgoingConnectionCount))
                ) && 
                (
                    this.RemoteHost == input.RemoteHost ||
                    (this.RemoteHost != null &&
                    this.RemoteHost.Equals(input.RemoteHost))
                ) && 
                (
                    this.RemotePort == input.RemotePort ||
                    (this.RemotePort != null &&
                    this.RemotePort.Equals(input.RemotePort))
                ) && 
                (
                    this.RestConsumerName == input.RestConsumerName ||
                    (this.RestConsumerName != null &&
                    this.RestConsumerName.Equals(input.RestConsumerName))
                ) && 
                (
                    this.RestDeliveryPointName == input.RestDeliveryPointName ||
                    (this.RestDeliveryPointName != null &&
                    this.RestDeliveryPointName.Equals(input.RestDeliveryPointName))
                ) && 
                (
                    this.RetryDelay == input.RetryDelay ||
                    (this.RetryDelay != null &&
                    this.RetryDelay.Equals(input.RetryDelay))
                ) && 
                (
                    this.TlsCipherSuiteList == input.TlsCipherSuiteList ||
                    (this.TlsCipherSuiteList != null &&
                    this.TlsCipherSuiteList.Equals(input.TlsCipherSuiteList))
                ) && 
                (
                    this.TlsEnabled == input.TlsEnabled ||
                    (this.TlsEnabled != null &&
                    this.TlsEnabled.Equals(input.TlsEnabled))
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
                if (this.AuthenticationAwsAccessKeyId != null)
                    hashCode = hashCode * 59 + this.AuthenticationAwsAccessKeyId.GetHashCode();
                if (this.AuthenticationAwsRegion != null)
                    hashCode = hashCode * 59 + this.AuthenticationAwsRegion.GetHashCode();
                if (this.AuthenticationAwsSecretAccessKey != null)
                    hashCode = hashCode * 59 + this.AuthenticationAwsSecretAccessKey.GetHashCode();
                if (this.AuthenticationAwsService != null)
                    hashCode = hashCode * 59 + this.AuthenticationAwsService.GetHashCode();
                if (this.AuthenticationClientCertContent != null)
                    hashCode = hashCode * 59 + this.AuthenticationClientCertContent.GetHashCode();
                if (this.AuthenticationClientCertPassword != null)
                    hashCode = hashCode * 59 + this.AuthenticationClientCertPassword.GetHashCode();
                if (this.AuthenticationHttpBasicPassword != null)
                    hashCode = hashCode * 59 + this.AuthenticationHttpBasicPassword.GetHashCode();
                if (this.AuthenticationHttpBasicUsername != null)
                    hashCode = hashCode * 59 + this.AuthenticationHttpBasicUsername.GetHashCode();
                if (this.AuthenticationHttpHeaderName != null)
                    hashCode = hashCode * 59 + this.AuthenticationHttpHeaderName.GetHashCode();
                if (this.AuthenticationHttpHeaderValue != null)
                    hashCode = hashCode * 59 + this.AuthenticationHttpHeaderValue.GetHashCode();
                if (this.AuthenticationOauthClientId != null)
                    hashCode = hashCode * 59 + this.AuthenticationOauthClientId.GetHashCode();
                if (this.AuthenticationOauthClientScope != null)
                    hashCode = hashCode * 59 + this.AuthenticationOauthClientScope.GetHashCode();
                if (this.AuthenticationOauthClientSecret != null)
                    hashCode = hashCode * 59 + this.AuthenticationOauthClientSecret.GetHashCode();
                if (this.AuthenticationOauthClientTokenEndpoint != null)
                    hashCode = hashCode * 59 + this.AuthenticationOauthClientTokenEndpoint.GetHashCode();
                if (this.AuthenticationOauthClientTokenExpiryDefault != null)
                    hashCode = hashCode * 59 + this.AuthenticationOauthClientTokenExpiryDefault.GetHashCode();
                if (this.AuthenticationOauthJwtSecretKey != null)
                    hashCode = hashCode * 59 + this.AuthenticationOauthJwtSecretKey.GetHashCode();
                if (this.AuthenticationOauthJwtTokenEndpoint != null)
                    hashCode = hashCode * 59 + this.AuthenticationOauthJwtTokenEndpoint.GetHashCode();
                if (this.AuthenticationOauthJwtTokenExpiryDefault != null)
                    hashCode = hashCode * 59 + this.AuthenticationOauthJwtTokenExpiryDefault.GetHashCode();
                if (this.AuthenticationScheme != null)
                    hashCode = hashCode * 59 + this.AuthenticationScheme.GetHashCode();
                if (this.Enabled != null)
                    hashCode = hashCode * 59 + this.Enabled.GetHashCode();
                if (this.HttpMethod != null)
                    hashCode = hashCode * 59 + this.HttpMethod.GetHashCode();
                if (this.LocalInterface != null)
                    hashCode = hashCode * 59 + this.LocalInterface.GetHashCode();
                if (this.MaxPostWaitTime != null)
                    hashCode = hashCode * 59 + this.MaxPostWaitTime.GetHashCode();
                if (this.MsgVpnName != null)
                    hashCode = hashCode * 59 + this.MsgVpnName.GetHashCode();
                if (this.OutgoingConnectionCount != null)
                    hashCode = hashCode * 59 + this.OutgoingConnectionCount.GetHashCode();
                if (this.RemoteHost != null)
                    hashCode = hashCode * 59 + this.RemoteHost.GetHashCode();
                if (this.RemotePort != null)
                    hashCode = hashCode * 59 + this.RemotePort.GetHashCode();
                if (this.RestConsumerName != null)
                    hashCode = hashCode * 59 + this.RestConsumerName.GetHashCode();
                if (this.RestDeliveryPointName != null)
                    hashCode = hashCode * 59 + this.RestDeliveryPointName.GetHashCode();
                if (this.RetryDelay != null)
                    hashCode = hashCode * 59 + this.RetryDelay.GetHashCode();
                if (this.TlsCipherSuiteList != null)
                    hashCode = hashCode * 59 + this.TlsCipherSuiteList.GetHashCode();
                if (this.TlsEnabled != null)
                    hashCode = hashCode * 59 + this.TlsEnabled.GetHashCode();
                return hashCode;
            }
        }
    }
}
