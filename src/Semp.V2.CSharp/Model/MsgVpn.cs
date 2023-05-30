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
    /// MsgVpn
    /// </summary>
    [DataContract]
        public partial class MsgVpn :  IEquatable<MsgVpn>
    {
        /// <summary>
        /// The type of basic authentication to use for clients connecting to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;radius\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;internal\&quot; - Internal database. Authentication is against Client Usernames. \&quot;ldap\&quot; - LDAP authentication. An LDAP profile name must be provided. \&quot;radius\&quot; - RADIUS authentication. A RADIUS profile name must be provided. \&quot;none\&quot; - No authentication. Anonymous login allowed. &lt;/pre&gt; 
        /// </summary>
        /// <value>The type of basic authentication to use for clients connecting to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;radius\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;internal\&quot; - Internal database. Authentication is against Client Usernames. \&quot;ldap\&quot; - LDAP authentication. An LDAP profile name must be provided. \&quot;radius\&quot; - RADIUS authentication. A RADIUS profile name must be provided. \&quot;none\&quot; - No authentication. Anonymous login allowed. &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum AuthenticationBasicTypeEnum
        {
            /// <summary>
            /// Enum Internal for value: internal
            /// </summary>
            [EnumMember(Value = "internal")]
            Internal = 1,
            /// <summary>
            /// Enum Ldap for value: ldap
            /// </summary>
            [EnumMember(Value = "ldap")]
            Ldap = 2,
            /// <summary>
            /// Enum Radius for value: radius
            /// </summary>
            [EnumMember(Value = "radius")]
            Radius = 3,
            /// <summary>
            /// Enum None for value: none
            /// </summary>
            [EnumMember(Value = "none")]
            None = 4        }
        /// <summary>
        /// The type of basic authentication to use for clients connecting to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;radius\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;internal\&quot; - Internal database. Authentication is against Client Usernames. \&quot;ldap\&quot; - LDAP authentication. An LDAP profile name must be provided. \&quot;radius\&quot; - RADIUS authentication. A RADIUS profile name must be provided. \&quot;none\&quot; - No authentication. Anonymous login allowed. &lt;/pre&gt; 
        /// </summary>
        /// <value>The type of basic authentication to use for clients connecting to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;radius\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;internal\&quot; - Internal database. Authentication is against Client Usernames. \&quot;ldap\&quot; - LDAP authentication. An LDAP profile name must be provided. \&quot;radius\&quot; - RADIUS authentication. A RADIUS profile name must be provided. \&quot;none\&quot; - No authentication. Anonymous login allowed. &lt;/pre&gt; </value>
        [DataMember(Name="authenticationBasicType", EmitDefaultValue=false)]
        public AuthenticationBasicTypeEnum? AuthenticationBasicType { get; set; }
        /// <summary>
        /// The desired behavior for client certificate revocation checking. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;allow-valid\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;allow-all\&quot; - Allow the client to authenticate, the result of client certificate revocation check is ignored. \&quot;allow-unknown\&quot; - Allow the client to authenticate even if the revocation status of his certificate cannot be determined. \&quot;allow-valid\&quot; - Allow the client to authenticate only when the revocation check returned an explicit positive response. &lt;/pre&gt;  Available since 2.6.
        /// </summary>
        /// <value>The desired behavior for client certificate revocation checking. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;allow-valid\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;allow-all\&quot; - Allow the client to authenticate, the result of client certificate revocation check is ignored. \&quot;allow-unknown\&quot; - Allow the client to authenticate even if the revocation status of his certificate cannot be determined. \&quot;allow-valid\&quot; - Allow the client to authenticate only when the revocation check returned an explicit positive response. &lt;/pre&gt;  Available since 2.6.</value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum AuthenticationClientCertRevocationCheckModeEnum
        {
            /// <summary>
            /// Enum All for value: allow-all
            /// </summary>
            [EnumMember(Value = "allow-all")]
            All = 1,
            /// <summary>
            /// Enum Unknown for value: allow-unknown
            /// </summary>
            [EnumMember(Value = "allow-unknown")]
            Unknown = 2,
            /// <summary>
            /// Enum Valid for value: allow-valid
            /// </summary>
            [EnumMember(Value = "allow-valid")]
            Valid = 3        }
        /// <summary>
        /// The desired behavior for client certificate revocation checking. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;allow-valid\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;allow-all\&quot; - Allow the client to authenticate, the result of client certificate revocation check is ignored. \&quot;allow-unknown\&quot; - Allow the client to authenticate even if the revocation status of his certificate cannot be determined. \&quot;allow-valid\&quot; - Allow the client to authenticate only when the revocation check returned an explicit positive response. &lt;/pre&gt;  Available since 2.6.
        /// </summary>
        /// <value>The desired behavior for client certificate revocation checking. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;allow-valid\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;allow-all\&quot; - Allow the client to authenticate, the result of client certificate revocation check is ignored. \&quot;allow-unknown\&quot; - Allow the client to authenticate even if the revocation status of his certificate cannot be determined. \&quot;allow-valid\&quot; - Allow the client to authenticate only when the revocation check returned an explicit positive response. &lt;/pre&gt;  Available since 2.6.</value>
        [DataMember(Name="authenticationClientCertRevocationCheckMode", EmitDefaultValue=false)]
        public AuthenticationClientCertRevocationCheckModeEnum? AuthenticationClientCertRevocationCheckMode { get; set; }
        /// <summary>
        /// The field from the client certificate to use as the client username. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;common-name\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;certificate-thumbprint\&quot; - The username is computed as the SHA-1 hash over the entire DER-encoded contents of the client certificate. \&quot;common-name\&quot; - The username is extracted from the certificate&#x27;s first instance of the Common Name attribute in the Subject DN. \&quot;common-name-last\&quot; - The username is extracted from the certificate&#x27;s last instance of the Common Name attribute in the Subject DN. \&quot;subject-alternate-name-msupn\&quot; - The username is extracted from the certificate&#x27;s Other Name type of the Subject Alternative Name and must have the msUPN signature. \&quot;uid\&quot; - The username is extracted from the certificate&#x27;s first instance of the User Identifier attribute in the Subject DN. \&quot;uid-last\&quot; - The username is extracted from the certificate&#x27;s last instance of the User Identifier attribute in the Subject DN. &lt;/pre&gt;  Available since 2.6.
        /// </summary>
        /// <value>The field from the client certificate to use as the client username. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;common-name\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;certificate-thumbprint\&quot; - The username is computed as the SHA-1 hash over the entire DER-encoded contents of the client certificate. \&quot;common-name\&quot; - The username is extracted from the certificate&#x27;s first instance of the Common Name attribute in the Subject DN. \&quot;common-name-last\&quot; - The username is extracted from the certificate&#x27;s last instance of the Common Name attribute in the Subject DN. \&quot;subject-alternate-name-msupn\&quot; - The username is extracted from the certificate&#x27;s Other Name type of the Subject Alternative Name and must have the msUPN signature. \&quot;uid\&quot; - The username is extracted from the certificate&#x27;s first instance of the User Identifier attribute in the Subject DN. \&quot;uid-last\&quot; - The username is extracted from the certificate&#x27;s last instance of the User Identifier attribute in the Subject DN. &lt;/pre&gt;  Available since 2.6.</value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum AuthenticationClientCertUsernameSourceEnum
        {
            /// <summary>
            /// Enum CertificateThumbprint for value: certificate-thumbprint
            /// </summary>
            [EnumMember(Value = "certificate-thumbprint")]
            CertificateThumbprint = 1,
            /// <summary>
            /// Enum CommonName for value: common-name
            /// </summary>
            [EnumMember(Value = "common-name")]
            CommonName = 2,
            /// <summary>
            /// Enum CommonNameLast for value: common-name-last
            /// </summary>
            [EnumMember(Value = "common-name-last")]
            CommonNameLast = 3,
            /// <summary>
            /// Enum SubjectAlternateNameMsupn for value: subject-alternate-name-msupn
            /// </summary>
            [EnumMember(Value = "subject-alternate-name-msupn")]
            SubjectAlternateNameMsupn = 4,
            /// <summary>
            /// Enum Uid for value: uid
            /// </summary>
            [EnumMember(Value = "uid")]
            Uid = 5,
            /// <summary>
            /// Enum UidLast for value: uid-last
            /// </summary>
            [EnumMember(Value = "uid-last")]
            UidLast = 6        }
        /// <summary>
        /// The field from the client certificate to use as the client username. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;common-name\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;certificate-thumbprint\&quot; - The username is computed as the SHA-1 hash over the entire DER-encoded contents of the client certificate. \&quot;common-name\&quot; - The username is extracted from the certificate&#x27;s first instance of the Common Name attribute in the Subject DN. \&quot;common-name-last\&quot; - The username is extracted from the certificate&#x27;s last instance of the Common Name attribute in the Subject DN. \&quot;subject-alternate-name-msupn\&quot; - The username is extracted from the certificate&#x27;s Other Name type of the Subject Alternative Name and must have the msUPN signature. \&quot;uid\&quot; - The username is extracted from the certificate&#x27;s first instance of the User Identifier attribute in the Subject DN. \&quot;uid-last\&quot; - The username is extracted from the certificate&#x27;s last instance of the User Identifier attribute in the Subject DN. &lt;/pre&gt;  Available since 2.6.
        /// </summary>
        /// <value>The field from the client certificate to use as the client username. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;common-name\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;certificate-thumbprint\&quot; - The username is computed as the SHA-1 hash over the entire DER-encoded contents of the client certificate. \&quot;common-name\&quot; - The username is extracted from the certificate&#x27;s first instance of the Common Name attribute in the Subject DN. \&quot;common-name-last\&quot; - The username is extracted from the certificate&#x27;s last instance of the Common Name attribute in the Subject DN. \&quot;subject-alternate-name-msupn\&quot; - The username is extracted from the certificate&#x27;s Other Name type of the Subject Alternative Name and must have the msUPN signature. \&quot;uid\&quot; - The username is extracted from the certificate&#x27;s first instance of the User Identifier attribute in the Subject DN. \&quot;uid-last\&quot; - The username is extracted from the certificate&#x27;s last instance of the User Identifier attribute in the Subject DN. &lt;/pre&gt;  Available since 2.6.</value>
        [DataMember(Name="authenticationClientCertUsernameSource", EmitDefaultValue=false)]
        public AuthenticationClientCertUsernameSourceEnum? AuthenticationClientCertUsernameSource { get; set; }
        /// <summary>
        /// The type of authorization to use for clients connecting to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;internal\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;ldap\&quot; - LDAP authorization. \&quot;internal\&quot; - Internal authorization. &lt;/pre&gt; 
        /// </summary>
        /// <value>The type of authorization to use for clients connecting to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;internal\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;ldap\&quot; - LDAP authorization. \&quot;internal\&quot; - Internal authorization. &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum AuthorizationTypeEnum
        {
            /// <summary>
            /// Enum Ldap for value: ldap
            /// </summary>
            [EnumMember(Value = "ldap")]
            Ldap = 1,
            /// <summary>
            /// Enum Internal for value: internal
            /// </summary>
            [EnumMember(Value = "internal")]
            Internal = 2        }
        /// <summary>
        /// The type of authorization to use for clients connecting to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;internal\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;ldap\&quot; - LDAP authorization. \&quot;internal\&quot; - Internal authorization. &lt;/pre&gt; 
        /// </summary>
        /// <value>The type of authorization to use for clients connecting to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;internal\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;ldap\&quot; - LDAP authorization. \&quot;internal\&quot; - Internal authorization. &lt;/pre&gt; </value>
        [DataMember(Name="authorizationType", EmitDefaultValue=false)]
        public AuthorizationTypeEnum? AuthorizationType { get; set; }
        /// <summary>
        /// Subscription level Event message publishing mode. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;off\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;off\&quot; - Disable client level event message publishing. \&quot;on-with-format-v1\&quot; - Enable client level event message publishing with format v1. \&quot;on-with-no-unsubscribe-events-on-disconnect-format-v1\&quot; - As \&quot;on-with-format-v1\&quot;, but unsubscribe events are not generated when a client disconnects. Unsubscribe events are still raised when a client explicitly unsubscribes from its subscriptions. \&quot;on-with-format-v2\&quot; - Enable client level event message publishing with format v2. \&quot;on-with-no-unsubscribe-events-on-disconnect-format-v2\&quot; - As \&quot;on-with-format-v2\&quot;, but unsubscribe events are not generated when a client disconnects. Unsubscribe events are still raised when a client explicitly unsubscribes from its subscriptions. &lt;/pre&gt; 
        /// </summary>
        /// <value>Subscription level Event message publishing mode. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;off\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;off\&quot; - Disable client level event message publishing. \&quot;on-with-format-v1\&quot; - Enable client level event message publishing with format v1. \&quot;on-with-no-unsubscribe-events-on-disconnect-format-v1\&quot; - As \&quot;on-with-format-v1\&quot;, but unsubscribe events are not generated when a client disconnects. Unsubscribe events are still raised when a client explicitly unsubscribes from its subscriptions. \&quot;on-with-format-v2\&quot; - Enable client level event message publishing with format v2. \&quot;on-with-no-unsubscribe-events-on-disconnect-format-v2\&quot; - As \&quot;on-with-format-v2\&quot;, but unsubscribe events are not generated when a client disconnects. Unsubscribe events are still raised when a client explicitly unsubscribes from its subscriptions. &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum EventPublishSubscriptionModeEnum
        {
            /// <summary>
            /// Enum Off for value: off
            /// </summary>
            [EnumMember(Value = "off")]
            Off = 1,
            /// <summary>
            /// Enum OnWithFormatV1 for value: on-with-format-v1
            /// </summary>
            [EnumMember(Value = "on-with-format-v1")]
            OnWithFormatV1 = 2,
            /// <summary>
            /// Enum OnWithNoUnsubscribeEventsOnDisconnectFormatV1 for value: on-with-no-unsubscribe-events-on-disconnect-format-v1
            /// </summary>
            [EnumMember(Value = "on-with-no-unsubscribe-events-on-disconnect-format-v1")]
            OnWithNoUnsubscribeEventsOnDisconnectFormatV1 = 3,
            /// <summary>
            /// Enum OnWithFormatV2 for value: on-with-format-v2
            /// </summary>
            [EnumMember(Value = "on-with-format-v2")]
            OnWithFormatV2 = 4,
            /// <summary>
            /// Enum OnWithNoUnsubscribeEventsOnDisconnectFormatV2 for value: on-with-no-unsubscribe-events-on-disconnect-format-v2
            /// </summary>
            [EnumMember(Value = "on-with-no-unsubscribe-events-on-disconnect-format-v2")]
            OnWithNoUnsubscribeEventsOnDisconnectFormatV2 = 5        }
        /// <summary>
        /// Subscription level Event message publishing mode. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;off\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;off\&quot; - Disable client level event message publishing. \&quot;on-with-format-v1\&quot; - Enable client level event message publishing with format v1. \&quot;on-with-no-unsubscribe-events-on-disconnect-format-v1\&quot; - As \&quot;on-with-format-v1\&quot;, but unsubscribe events are not generated when a client disconnects. Unsubscribe events are still raised when a client explicitly unsubscribes from its subscriptions. \&quot;on-with-format-v2\&quot; - Enable client level event message publishing with format v2. \&quot;on-with-no-unsubscribe-events-on-disconnect-format-v2\&quot; - As \&quot;on-with-format-v2\&quot;, but unsubscribe events are not generated when a client disconnects. Unsubscribe events are still raised when a client explicitly unsubscribes from its subscriptions. &lt;/pre&gt; 
        /// </summary>
        /// <value>Subscription level Event message publishing mode. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;off\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;off\&quot; - Disable client level event message publishing. \&quot;on-with-format-v1\&quot; - Enable client level event message publishing with format v1. \&quot;on-with-no-unsubscribe-events-on-disconnect-format-v1\&quot; - As \&quot;on-with-format-v1\&quot;, but unsubscribe events are not generated when a client disconnects. Unsubscribe events are still raised when a client explicitly unsubscribes from its subscriptions. \&quot;on-with-format-v2\&quot; - Enable client level event message publishing with format v2. \&quot;on-with-no-unsubscribe-events-on-disconnect-format-v2\&quot; - As \&quot;on-with-format-v2\&quot;, but unsubscribe events are not generated when a client disconnects. Unsubscribe events are still raised when a client explicitly unsubscribes from its subscriptions. &lt;/pre&gt; </value>
        [DataMember(Name="eventPublishSubscriptionMode", EmitDefaultValue=false)]
        public EventPublishSubscriptionModeEnum? EventPublishSubscriptionMode { get; set; }
        /// <summary>
        /// The authentication scheme for the replication Bridge in the Message VPN. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;basic\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;basic\&quot; - Basic Authentication Scheme (via username and password). \&quot;client-certificate\&quot; - Client Certificate Authentication Scheme (via certificate file or content). &lt;/pre&gt; 
        /// </summary>
        /// <value>The authentication scheme for the replication Bridge in the Message VPN. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;basic\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;basic\&quot; - Basic Authentication Scheme (via username and password). \&quot;client-certificate\&quot; - Client Certificate Authentication Scheme (via certificate file or content). &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum ReplicationBridgeAuthenticationSchemeEnum
        {
            /// <summary>
            /// Enum Basic for value: basic
            /// </summary>
            [EnumMember(Value = "basic")]
            Basic = 1,
            /// <summary>
            /// Enum ClientCertificate for value: client-certificate
            /// </summary>
            [EnumMember(Value = "client-certificate")]
            ClientCertificate = 2        }
        /// <summary>
        /// The authentication scheme for the replication Bridge in the Message VPN. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;basic\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;basic\&quot; - Basic Authentication Scheme (via username and password). \&quot;client-certificate\&quot; - Client Certificate Authentication Scheme (via certificate file or content). &lt;/pre&gt; 
        /// </summary>
        /// <value>The authentication scheme for the replication Bridge in the Message VPN. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;basic\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;basic\&quot; - Basic Authentication Scheme (via username and password). \&quot;client-certificate\&quot; - Client Certificate Authentication Scheme (via certificate file or content). &lt;/pre&gt; </value>
        [DataMember(Name="replicationBridgeAuthenticationScheme", EmitDefaultValue=false)]
        public ReplicationBridgeAuthenticationSchemeEnum? ReplicationBridgeAuthenticationScheme { get; set; }
        /// <summary>
        /// The behavior to take when enabling replication for the Message VPN, depending on the existence of the replication Queue. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;fail-on-existing-queue\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;fail-on-existing-queue\&quot; - The data replication queue must not already exist. \&quot;force-use-existing-queue\&quot; - The data replication queue must already exist. Any data messages on the Queue will be forwarded to interested applications. IMPORTANT: Before using this mode be certain that the messages are not stale or otherwise unsuitable to be forwarded. This mode can only be specified when the existing queue is configured the same as is currently specified under replication configuration otherwise the enabling of replication will fail. \&quot;force-recreate-queue\&quot; - The data replication queue must already exist. Any data messages on the Queue will be discarded. IMPORTANT: Before using this mode be certain that the messages on the existing data replication queue are not needed by interested applications. &lt;/pre&gt; 
        /// </summary>
        /// <value>The behavior to take when enabling replication for the Message VPN, depending on the existence of the replication Queue. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;fail-on-existing-queue\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;fail-on-existing-queue\&quot; - The data replication queue must not already exist. \&quot;force-use-existing-queue\&quot; - The data replication queue must already exist. Any data messages on the Queue will be forwarded to interested applications. IMPORTANT: Before using this mode be certain that the messages are not stale or otherwise unsuitable to be forwarded. This mode can only be specified when the existing queue is configured the same as is currently specified under replication configuration otherwise the enabling of replication will fail. \&quot;force-recreate-queue\&quot; - The data replication queue must already exist. Any data messages on the Queue will be discarded. IMPORTANT: Before using this mode be certain that the messages on the existing data replication queue are not needed by interested applications. &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum ReplicationEnabledQueueBehaviorEnum
        {
            /// <summary>
            /// Enum FailOnExistingQueue for value: fail-on-existing-queue
            /// </summary>
            [EnumMember(Value = "fail-on-existing-queue")]
            FailOnExistingQueue = 1,
            /// <summary>
            /// Enum ForceUseExistingQueue for value: force-use-existing-queue
            /// </summary>
            [EnumMember(Value = "force-use-existing-queue")]
            ForceUseExistingQueue = 2,
            /// <summary>
            /// Enum ForceRecreateQueue for value: force-recreate-queue
            /// </summary>
            [EnumMember(Value = "force-recreate-queue")]
            ForceRecreateQueue = 3        }
        /// <summary>
        /// The behavior to take when enabling replication for the Message VPN, depending on the existence of the replication Queue. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;fail-on-existing-queue\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;fail-on-existing-queue\&quot; - The data replication queue must not already exist. \&quot;force-use-existing-queue\&quot; - The data replication queue must already exist. Any data messages on the Queue will be forwarded to interested applications. IMPORTANT: Before using this mode be certain that the messages are not stale or otherwise unsuitable to be forwarded. This mode can only be specified when the existing queue is configured the same as is currently specified under replication configuration otherwise the enabling of replication will fail. \&quot;force-recreate-queue\&quot; - The data replication queue must already exist. Any data messages on the Queue will be discarded. IMPORTANT: Before using this mode be certain that the messages on the existing data replication queue are not needed by interested applications. &lt;/pre&gt; 
        /// </summary>
        /// <value>The behavior to take when enabling replication for the Message VPN, depending on the existence of the replication Queue. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;fail-on-existing-queue\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;fail-on-existing-queue\&quot; - The data replication queue must not already exist. \&quot;force-use-existing-queue\&quot; - The data replication queue must already exist. Any data messages on the Queue will be forwarded to interested applications. IMPORTANT: Before using this mode be certain that the messages are not stale or otherwise unsuitable to be forwarded. This mode can only be specified when the existing queue is configured the same as is currently specified under replication configuration otherwise the enabling of replication will fail. \&quot;force-recreate-queue\&quot; - The data replication queue must already exist. Any data messages on the Queue will be discarded. IMPORTANT: Before using this mode be certain that the messages on the existing data replication queue are not needed by interested applications. &lt;/pre&gt; </value>
        [DataMember(Name="replicationEnabledQueueBehavior", EmitDefaultValue=false)]
        public ReplicationEnabledQueueBehaviorEnum? ReplicationEnabledQueueBehavior { get; set; }
        /// <summary>
        /// The replication role for the Message VPN. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;standby\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;active\&quot; - Assume the Active role in replication for the Message VPN. \&quot;standby\&quot; - Assume the Standby role in replication for the Message VPN. &lt;/pre&gt; 
        /// </summary>
        /// <value>The replication role for the Message VPN. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;standby\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;active\&quot; - Assume the Active role in replication for the Message VPN. \&quot;standby\&quot; - Assume the Standby role in replication for the Message VPN. &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum ReplicationRoleEnum
        {
            /// <summary>
            /// Enum Active for value: active
            /// </summary>
            [EnumMember(Value = "active")]
            Active = 1,
            /// <summary>
            /// Enum Standby for value: standby
            /// </summary>
            [EnumMember(Value = "standby")]
            Standby = 2        }
        /// <summary>
        /// The replication role for the Message VPN. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;standby\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;active\&quot; - Assume the Active role in replication for the Message VPN. \&quot;standby\&quot; - Assume the Standby role in replication for the Message VPN. &lt;/pre&gt; 
        /// </summary>
        /// <value>The replication role for the Message VPN. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;standby\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;active\&quot; - Assume the Active role in replication for the Message VPN. \&quot;standby\&quot; - Assume the Standby role in replication for the Message VPN. &lt;/pre&gt; </value>
        [DataMember(Name="replicationRole", EmitDefaultValue=false)]
        public ReplicationRoleEnum? ReplicationRole { get; set; }
        /// <summary>
        /// The transaction replication mode for all transactions within the Message VPN. Changing this value during operation will not affect existing transactions; it is only used upon starting a transaction. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;async\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;sync\&quot; - Messages are acknowledged when replicated (spooled remotely). \&quot;async\&quot; - Messages are acknowledged when pending replication (spooled locally). &lt;/pre&gt; 
        /// </summary>
        /// <value>The transaction replication mode for all transactions within the Message VPN. Changing this value during operation will not affect existing transactions; it is only used upon starting a transaction. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;async\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;sync\&quot; - Messages are acknowledged when replicated (spooled remotely). \&quot;async\&quot; - Messages are acknowledged when pending replication (spooled locally). &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum ReplicationTransactionModeEnum
        {
            /// <summary>
            /// Enum Sync for value: sync
            /// </summary>
            [EnumMember(Value = "sync")]
            Sync = 1,
            /// <summary>
            /// Enum Async for value: async
            /// </summary>
            [EnumMember(Value = "async")]
            Async = 2        }
        /// <summary>
        /// The transaction replication mode for all transactions within the Message VPN. Changing this value during operation will not affect existing transactions; it is only used upon starting a transaction. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;async\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;sync\&quot; - Messages are acknowledged when replicated (spooled remotely). \&quot;async\&quot; - Messages are acknowledged when pending replication (spooled locally). &lt;/pre&gt; 
        /// </summary>
        /// <value>The transaction replication mode for all transactions within the Message VPN. Changing this value during operation will not affect existing transactions; it is only used upon starting a transaction. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;async\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;sync\&quot; - Messages are acknowledged when replicated (spooled remotely). \&quot;async\&quot; - Messages are acknowledged when pending replication (spooled locally). &lt;/pre&gt; </value>
        [DataMember(Name="replicationTransactionMode", EmitDefaultValue=false)]
        public ReplicationTransactionModeEnum? ReplicationTransactionMode { get; set; }
        /// <summary>
        /// Determines when to request a client certificate from an incoming MQTT client connecting via a TLS port. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-enabled-in-message-vpn\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;never\&quot; - Never ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;when-enabled-in-message-vpn\&quot; - Only ask for a client-certificate if client certificate authentication is enabled under \&quot;message-vpn &gt;  authentication &gt; client-certificate &gt; shutdown\&quot;. &lt;/pre&gt;  Available since 2.21.
        /// </summary>
        /// <value>Determines when to request a client certificate from an incoming MQTT client connecting via a TLS port. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-enabled-in-message-vpn\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;never\&quot; - Never ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;when-enabled-in-message-vpn\&quot; - Only ask for a client-certificate if client certificate authentication is enabled under \&quot;message-vpn &gt;  authentication &gt; client-certificate &gt; shutdown\&quot;. &lt;/pre&gt;  Available since 2.21.</value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum ServiceMqttAuthenticationClientCertRequestEnum
        {
            /// <summary>
            /// Enum Always for value: always
            /// </summary>
            [EnumMember(Value = "always")]
            Always = 1,
            /// <summary>
            /// Enum Never for value: never
            /// </summary>
            [EnumMember(Value = "never")]
            Never = 2,
            /// <summary>
            /// Enum WhenEnabledInMessageVpn for value: when-enabled-in-message-vpn
            /// </summary>
            [EnumMember(Value = "when-enabled-in-message-vpn")]
            WhenEnabledInMessageVpn = 3        }
        /// <summary>
        /// Determines when to request a client certificate from an incoming MQTT client connecting via a TLS port. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-enabled-in-message-vpn\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;never\&quot; - Never ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;when-enabled-in-message-vpn\&quot; - Only ask for a client-certificate if client certificate authentication is enabled under \&quot;message-vpn &gt;  authentication &gt; client-certificate &gt; shutdown\&quot;. &lt;/pre&gt;  Available since 2.21.
        /// </summary>
        /// <value>Determines when to request a client certificate from an incoming MQTT client connecting via a TLS port. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-enabled-in-message-vpn\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;never\&quot; - Never ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;when-enabled-in-message-vpn\&quot; - Only ask for a client-certificate if client certificate authentication is enabled under \&quot;message-vpn &gt;  authentication &gt; client-certificate &gt; shutdown\&quot;. &lt;/pre&gt;  Available since 2.21.</value>
        [DataMember(Name="serviceMqttAuthenticationClientCertRequest", EmitDefaultValue=false)]
        public ServiceMqttAuthenticationClientCertRequestEnum? ServiceMqttAuthenticationClientCertRequest { get; set; }
        /// <summary>
        /// Determines when to request a client certificate from an incoming REST Producer connecting via a TLS port. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-enabled-in-message-vpn\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;never\&quot; - Never ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;when-enabled-in-message-vpn\&quot; - Only ask for a client-certificate if client certificate authentication is enabled under \&quot;message-vpn &gt;  authentication &gt; client-certificate &gt; shutdown\&quot;. &lt;/pre&gt;  Available since 2.21.
        /// </summary>
        /// <value>Determines when to request a client certificate from an incoming REST Producer connecting via a TLS port. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-enabled-in-message-vpn\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;never\&quot; - Never ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;when-enabled-in-message-vpn\&quot; - Only ask for a client-certificate if client certificate authentication is enabled under \&quot;message-vpn &gt;  authentication &gt; client-certificate &gt; shutdown\&quot;. &lt;/pre&gt;  Available since 2.21.</value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum ServiceRestIncomingAuthenticationClientCertRequestEnum
        {
            /// <summary>
            /// Enum Always for value: always
            /// </summary>
            [EnumMember(Value = "always")]
            Always = 1,
            /// <summary>
            /// Enum Never for value: never
            /// </summary>
            [EnumMember(Value = "never")]
            Never = 2,
            /// <summary>
            /// Enum WhenEnabledInMessageVpn for value: when-enabled-in-message-vpn
            /// </summary>
            [EnumMember(Value = "when-enabled-in-message-vpn")]
            WhenEnabledInMessageVpn = 3        }
        /// <summary>
        /// Determines when to request a client certificate from an incoming REST Producer connecting via a TLS port. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-enabled-in-message-vpn\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;never\&quot; - Never ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;when-enabled-in-message-vpn\&quot; - Only ask for a client-certificate if client certificate authentication is enabled under \&quot;message-vpn &gt;  authentication &gt; client-certificate &gt; shutdown\&quot;. &lt;/pre&gt;  Available since 2.21.
        /// </summary>
        /// <value>Determines when to request a client certificate from an incoming REST Producer connecting via a TLS port. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-enabled-in-message-vpn\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;never\&quot; - Never ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;when-enabled-in-message-vpn\&quot; - Only ask for a client-certificate if client certificate authentication is enabled under \&quot;message-vpn &gt;  authentication &gt; client-certificate &gt; shutdown\&quot;. &lt;/pre&gt;  Available since 2.21.</value>
        [DataMember(Name="serviceRestIncomingAuthenticationClientCertRequest", EmitDefaultValue=false)]
        public ServiceRestIncomingAuthenticationClientCertRequestEnum? ServiceRestIncomingAuthenticationClientCertRequest { get; set; }
        /// <summary>
        /// The handling of Authorization headers for incoming REST connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;drop\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;drop\&quot; - Do not attach the Authorization header to the message as a user property. This configuration is most secure. \&quot;forward\&quot; - Forward the Authorization header, attaching it to the message as a user property in the same way as other headers. For best security, use the drop setting. \&quot;legacy\&quot; - If the Authorization header was used for authentication to the broker, do not attach it to the message. If the Authorization header was not used for authentication to the broker, attach it to the message as a user property in the same way as other headers. For best security, use the drop setting. &lt;/pre&gt;  Available since 2.19.
        /// </summary>
        /// <value>The handling of Authorization headers for incoming REST connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;drop\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;drop\&quot; - Do not attach the Authorization header to the message as a user property. This configuration is most secure. \&quot;forward\&quot; - Forward the Authorization header, attaching it to the message as a user property in the same way as other headers. For best security, use the drop setting. \&quot;legacy\&quot; - If the Authorization header was used for authentication to the broker, do not attach it to the message. If the Authorization header was not used for authentication to the broker, attach it to the message as a user property in the same way as other headers. For best security, use the drop setting. &lt;/pre&gt;  Available since 2.19.</value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum ServiceRestIncomingAuthorizationHeaderHandlingEnum
        {
            /// <summary>
            /// Enum Drop for value: drop
            /// </summary>
            [EnumMember(Value = "drop")]
            Drop = 1,
            /// <summary>
            /// Enum Forward for value: forward
            /// </summary>
            [EnumMember(Value = "forward")]
            Forward = 2,
            /// <summary>
            /// Enum Legacy for value: legacy
            /// </summary>
            [EnumMember(Value = "legacy")]
            Legacy = 3        }
        /// <summary>
        /// The handling of Authorization headers for incoming REST connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;drop\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;drop\&quot; - Do not attach the Authorization header to the message as a user property. This configuration is most secure. \&quot;forward\&quot; - Forward the Authorization header, attaching it to the message as a user property in the same way as other headers. For best security, use the drop setting. \&quot;legacy\&quot; - If the Authorization header was used for authentication to the broker, do not attach it to the message. If the Authorization header was not used for authentication to the broker, attach it to the message as a user property in the same way as other headers. For best security, use the drop setting. &lt;/pre&gt;  Available since 2.19.
        /// </summary>
        /// <value>The handling of Authorization headers for incoming REST connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;drop\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;drop\&quot; - Do not attach the Authorization header to the message as a user property. This configuration is most secure. \&quot;forward\&quot; - Forward the Authorization header, attaching it to the message as a user property in the same way as other headers. For best security, use the drop setting. \&quot;legacy\&quot; - If the Authorization header was used for authentication to the broker, do not attach it to the message. If the Authorization header was not used for authentication to the broker, attach it to the message as a user property in the same way as other headers. For best security, use the drop setting. &lt;/pre&gt;  Available since 2.19.</value>
        [DataMember(Name="serviceRestIncomingAuthorizationHeaderHandling", EmitDefaultValue=false)]
        public ServiceRestIncomingAuthorizationHeaderHandlingEnum? ServiceRestIncomingAuthorizationHeaderHandling { get; set; }
        /// <summary>
        /// The REST service mode for incoming REST clients that connect to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;messaging\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;gateway\&quot; - Act as a message gateway through which REST messages are propagated. \&quot;messaging\&quot; - Act as a message broker on which REST messages are queued. &lt;/pre&gt;  Available since 2.6.
        /// </summary>
        /// <value>The REST service mode for incoming REST clients that connect to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;messaging\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;gateway\&quot; - Act as a message gateway through which REST messages are propagated. \&quot;messaging\&quot; - Act as a message broker on which REST messages are queued. &lt;/pre&gt;  Available since 2.6.</value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum ServiceRestModeEnum
        {
            /// <summary>
            /// Enum Gateway for value: gateway
            /// </summary>
            [EnumMember(Value = "gateway")]
            Gateway = 1,
            /// <summary>
            /// Enum Messaging for value: messaging
            /// </summary>
            [EnumMember(Value = "messaging")]
            Messaging = 2        }
        /// <summary>
        /// The REST service mode for incoming REST clients that connect to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;messaging\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;gateway\&quot; - Act as a message gateway through which REST messages are propagated. \&quot;messaging\&quot; - Act as a message broker on which REST messages are queued. &lt;/pre&gt;  Available since 2.6.
        /// </summary>
        /// <value>The REST service mode for incoming REST clients that connect to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;messaging\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;gateway\&quot; - Act as a message gateway through which REST messages are propagated. \&quot;messaging\&quot; - Act as a message broker on which REST messages are queued. &lt;/pre&gt;  Available since 2.6.</value>
        [DataMember(Name="serviceRestMode", EmitDefaultValue=false)]
        public ServiceRestModeEnum? ServiceRestMode { get; set; }
        /// <summary>
        /// Determines when to request a client certificate from a Web Transport client connecting via a TLS port. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-enabled-in-message-vpn\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;never\&quot; - Never ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;when-enabled-in-message-vpn\&quot; - Only ask for a client-certificate if client certificate authentication is enabled under \&quot;message-vpn &gt;  authentication &gt; client-certificate &gt; shutdown\&quot;. &lt;/pre&gt;  Available since 2.21.
        /// </summary>
        /// <value>Determines when to request a client certificate from a Web Transport client connecting via a TLS port. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-enabled-in-message-vpn\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;never\&quot; - Never ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;when-enabled-in-message-vpn\&quot; - Only ask for a client-certificate if client certificate authentication is enabled under \&quot;message-vpn &gt;  authentication &gt; client-certificate &gt; shutdown\&quot;. &lt;/pre&gt;  Available since 2.21.</value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum ServiceWebAuthenticationClientCertRequestEnum
        {
            /// <summary>
            /// Enum Always for value: always
            /// </summary>
            [EnumMember(Value = "always")]
            Always = 1,
            /// <summary>
            /// Enum Never for value: never
            /// </summary>
            [EnumMember(Value = "never")]
            Never = 2,
            /// <summary>
            /// Enum WhenEnabledInMessageVpn for value: when-enabled-in-message-vpn
            /// </summary>
            [EnumMember(Value = "when-enabled-in-message-vpn")]
            WhenEnabledInMessageVpn = 3        }
        /// <summary>
        /// Determines when to request a client certificate from a Web Transport client connecting via a TLS port. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-enabled-in-message-vpn\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;never\&quot; - Never ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;when-enabled-in-message-vpn\&quot; - Only ask for a client-certificate if client certificate authentication is enabled under \&quot;message-vpn &gt;  authentication &gt; client-certificate &gt; shutdown\&quot;. &lt;/pre&gt;  Available since 2.21.
        /// </summary>
        /// <value>Determines when to request a client certificate from a Web Transport client connecting via a TLS port. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-enabled-in-message-vpn\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;never\&quot; - Never ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;when-enabled-in-message-vpn\&quot; - Only ask for a client-certificate if client certificate authentication is enabled under \&quot;message-vpn &gt;  authentication &gt; client-certificate &gt; shutdown\&quot;. &lt;/pre&gt;  Available since 2.21.</value>
        [DataMember(Name="serviceWebAuthenticationClientCertRequest", EmitDefaultValue=false)]
        public ServiceWebAuthenticationClientCertRequestEnum? ServiceWebAuthenticationClientCertRequest { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="MsgVpn" /> class.
        /// </summary>
        /// <param name="alias">The name of another Message VPN which this Message VPN is an alias for. When this Message VPN is enabled, the alias has no effect. When this Message VPN is disabled, Clients (but not Bridges and routing Links) logging into this Message VPN are automatically logged in to the other Message VPN, and authentication and authorization take place in the context of the other Message VPN.  Aliases may form a non-circular chain, cascading one to the next. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.14..</param>
        /// <param name="authenticationBasicEnabled">Enable or disable basic authentication for clients connecting to the Message VPN. Basic authentication is authentication that involves the use of a username and password to prove identity. If a user provides credentials for a different authentication scheme, this setting is not applicable. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="authenticationBasicProfileName">The name of the RADIUS or LDAP Profile to use for basic authentication. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;..</param>
        /// <param name="authenticationBasicRadiusDomain">The RADIUS domain to use for basic authentication. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="authenticationBasicType">The type of basic authentication to use for clients connecting to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;radius\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;internal\&quot; - Internal database. Authentication is against Client Usernames. \&quot;ldap\&quot; - LDAP authentication. An LDAP profile name must be provided. \&quot;radius\&quot; - RADIUS authentication. A RADIUS profile name must be provided. \&quot;none\&quot; - No authentication. Anonymous login allowed. &lt;/pre&gt; .</param>
        /// <param name="authenticationClientCertAllowApiProvidedUsernameEnabled">Enable or disable allowing a client to specify a Client Username via the API connect method. When disabled, the certificate CN (Common Name) is always used. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="authenticationClientCertCertificateMatchingRulesEnabled">Enable or disable certificate matching rules. When disabled, any valid certificate is accepted. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.27..</param>
        /// <param name="authenticationClientCertEnabled">Enable or disable client certificate authentication in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="authenticationClientCertMaxChainDepth">The maximum depth for a client certificate chain. The depth of a chain is defined as the number of signing CA certificates that are present in the chain back to a trusted self-signed root CA certificate. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;..</param>
        /// <param name="authenticationClientCertRevocationCheckMode">The desired behavior for client certificate revocation checking. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;allow-valid\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;allow-all\&quot; - Allow the client to authenticate, the result of client certificate revocation check is ignored. \&quot;allow-unknown\&quot; - Allow the client to authenticate even if the revocation status of his certificate cannot be determined. \&quot;allow-valid\&quot; - Allow the client to authenticate only when the revocation check returned an explicit positive response. &lt;/pre&gt;  Available since 2.6..</param>
        /// <param name="authenticationClientCertUsernameSource">The field from the client certificate to use as the client username. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;common-name\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;certificate-thumbprint\&quot; - The username is computed as the SHA-1 hash over the entire DER-encoded contents of the client certificate. \&quot;common-name\&quot; - The username is extracted from the certificate&#x27;s first instance of the Common Name attribute in the Subject DN. \&quot;common-name-last\&quot; - The username is extracted from the certificate&#x27;s last instance of the Common Name attribute in the Subject DN. \&quot;subject-alternate-name-msupn\&quot; - The username is extracted from the certificate&#x27;s Other Name type of the Subject Alternative Name and must have the msUPN signature. \&quot;uid\&quot; - The username is extracted from the certificate&#x27;s first instance of the User Identifier attribute in the Subject DN. \&quot;uid-last\&quot; - The username is extracted from the certificate&#x27;s last instance of the User Identifier attribute in the Subject DN. &lt;/pre&gt;  Available since 2.6..</param>
        /// <param name="authenticationClientCertValidateDateEnabled">Enable or disable validation of the \&quot;Not Before\&quot; and \&quot;Not After\&quot; validity dates in the client certificate. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="authenticationKerberosAllowApiProvidedUsernameEnabled">Enable or disable allowing a client to specify a Client Username via the API connect method. When disabled, the Kerberos Principal name is always used. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="authenticationKerberosEnabled">Enable or disable Kerberos authentication in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="authenticationOauthDefaultProfileName">The name of the profile to use when the client does not supply a profile name. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.25..</param>
        /// <param name="authenticationOauthDefaultProviderName">The name of the provider to use when the client does not supply a provider name. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.25. authenticationOauthDefaultProviderName and authenticationOauthProviders replaced by authenticationOauthDefaultProfileName and authenticationOauthProfiles..</param>
        /// <param name="authenticationOauthEnabled">Enable or disable OAuth authentication. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.13..</param>
        /// <param name="authorizationLdapGroupMembershipAttributeName">The name of the attribute that is retrieved from the LDAP server as part of the LDAP search when authorizing a client connecting to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;memberOf\&quot;&#x60;..</param>
        /// <param name="authorizationLdapTrimClientUsernameDomainEnabled">Enable or disable client-username domain trimming for LDAP lookups of client connections. When enabled, the value of $CLIENT_USERNAME (when used for searching) will be truncated at the first occurance of the @ character. For example, if the client-username is in the form of an email address, then the domain portion will be removed. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.13..</param>
        /// <param name="authorizationProfileName">The name of the LDAP Profile to use for client authorization. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="authorizationType">The type of authorization to use for clients connecting to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;internal\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;ldap\&quot; - LDAP authorization. \&quot;internal\&quot; - Internal authorization. &lt;/pre&gt; .</param>
        /// <param name="bridgingTlsServerCertEnforceTrustedCommonNameEnabled">Enable or disable validation of the Common Name (CN) in the server certificate from the remote broker. If enabled, the Common Name is checked against the list of Trusted Common Names configured for the Bridge. Common Name validation is not performed if Server Certificate Name Validation is enabled, even if Common Name validation is enabled. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation..</param>
        /// <param name="bridgingTlsServerCertMaxChainDepth">The maximum depth for a server certificate chain. The depth of a chain is defined as the number of signing CA certificates that are present in the chain back to a trusted self-signed root CA certificate. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;..</param>
        /// <param name="bridgingTlsServerCertValidateDateEnabled">Enable or disable validation of the \&quot;Not Before\&quot; and \&quot;Not After\&quot; validity dates in the server certificate. When disabled, a certificate will be accepted even if the certificate is not valid based on these dates. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="bridgingTlsServerCertValidateNameEnabled">Enable or disable the standard TLS authentication mechanism of verifying the name used to connect to the bridge. If enabled, the name used to connect to the bridge is checked against the names specified in the certificate returned by the remote router. Legacy Common Name validation is not performed if Server Certificate Name Validation is enabled, even if Common Name validation is also enabled. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;. Available since 2.18..</param>
        /// <param name="distributedCacheManagementEnabled">Enable or disable managing of cache instances over the message bus. The default value is &#x60;true&#x60;. Deprecated since 2.28. Distributed cache mangement is now redundancy aware and thus no longer requires administrative intervention for operational state..</param>
        /// <param name="dmrEnabled">Enable or disable Dynamic Message Routing (DMR) for the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.11..</param>
        /// <param name="enabled">Enable or disable the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="eventConnectionCountThreshold">eventConnectionCountThreshold.</param>
        /// <param name="eventEgressFlowCountThreshold">eventEgressFlowCountThreshold.</param>
        /// <param name="eventEgressMsgRateThreshold">eventEgressMsgRateThreshold.</param>
        /// <param name="eventEndpointCountThreshold">eventEndpointCountThreshold.</param>
        /// <param name="eventIngressFlowCountThreshold">eventIngressFlowCountThreshold.</param>
        /// <param name="eventIngressMsgRateThreshold">eventIngressMsgRateThreshold.</param>
        /// <param name="eventLargeMsgThreshold">The threshold, in kilobytes, after which a message is considered to be large for the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1024&#x60;..</param>
        /// <param name="eventLogTag">A prefix applied to all published Events in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="eventMsgSpoolUsageThreshold">eventMsgSpoolUsageThreshold.</param>
        /// <param name="eventPublishClientEnabled">Enable or disable Client level Event message publishing. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="eventPublishMsgVpnEnabled">Enable or disable Message VPN level Event message publishing. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="eventPublishSubscriptionMode">Subscription level Event message publishing mode. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;off\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;off\&quot; - Disable client level event message publishing. \&quot;on-with-format-v1\&quot; - Enable client level event message publishing with format v1. \&quot;on-with-no-unsubscribe-events-on-disconnect-format-v1\&quot; - As \&quot;on-with-format-v1\&quot;, but unsubscribe events are not generated when a client disconnects. Unsubscribe events are still raised when a client explicitly unsubscribes from its subscriptions. \&quot;on-with-format-v2\&quot; - Enable client level event message publishing with format v2. \&quot;on-with-no-unsubscribe-events-on-disconnect-format-v2\&quot; - As \&quot;on-with-format-v2\&quot;, but unsubscribe events are not generated when a client disconnects. Unsubscribe events are still raised when a client explicitly unsubscribes from its subscriptions. &lt;/pre&gt; .</param>
        /// <param name="eventPublishTopicFormatMqttEnabled">Enable or disable Event publish topics in MQTT format. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="eventPublishTopicFormatSmfEnabled">Enable or disable Event publish topics in SMF format. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="eventServiceAmqpConnectionCountThreshold">eventServiceAmqpConnectionCountThreshold.</param>
        /// <param name="eventServiceMqttConnectionCountThreshold">eventServiceMqttConnectionCountThreshold.</param>
        /// <param name="eventServiceRestIncomingConnectionCountThreshold">eventServiceRestIncomingConnectionCountThreshold.</param>
        /// <param name="eventServiceSmfConnectionCountThreshold">eventServiceSmfConnectionCountThreshold.</param>
        /// <param name="eventServiceWebConnectionCountThreshold">eventServiceWebConnectionCountThreshold.</param>
        /// <param name="eventSubscriptionCountThreshold">eventSubscriptionCountThreshold.</param>
        /// <param name="eventTransactedSessionCountThreshold">eventTransactedSessionCountThreshold.</param>
        /// <param name="eventTransactionCountThreshold">eventTransactionCountThreshold.</param>
        /// <param name="exportSubscriptionsEnabled">Enable or disable the export of subscriptions in the Message VPN to other routers in the network over Neighbor links. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="jndiEnabled">Enable or disable JNDI access for clients in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.2..</param>
        /// <param name="maxConnectionCount">The maximum number of client connections to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform..</param>
        /// <param name="maxEgressFlowCount">The maximum number of transmit flows that can be created in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;..</param>
        /// <param name="maxEndpointCount">The maximum number of Queues and Topic Endpoints that can be created in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;..</param>
        /// <param name="maxIngressFlowCount">The maximum number of receive flows that can be created in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;..</param>
        /// <param name="maxMsgSpoolUsage">The maximum message spool usage by the Message VPN, in megabytes. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;..</param>
        /// <param name="maxSubscriptionCount">The maximum number of local client subscriptions that can be added to the Message VPN. This limit is not enforced when a subscription is added using a management interface, such as CLI or SEMP. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default varies by platform..</param>
        /// <param name="maxTransactedSessionCount">The maximum number of transacted sessions that can be created in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default varies by platform..</param>
        /// <param name="maxTransactionCount">The maximum number of transactions that can be created in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default varies by platform..</param>
        /// <param name="mqttRetainMaxMemory">The maximum total memory usage of the MQTT Retain feature for this Message VPN, in MB. If the maximum memory is reached, any arriving retain messages that require more memory are discarded. A value of -1 indicates that the memory is bounded only by the global max memory limit. A value of 0 prevents MQTT Retain from becoming operational. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;-1&#x60;. Available since 2.11..</param>
        /// <param name="msgVpnName">The name of the Message VPN..</param>
        /// <param name="replicationAckPropagationIntervalMsgCount">The acknowledgement (ACK) propagation interval for the replication Bridge, in number of replicated messages. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;20&#x60;..</param>
        /// <param name="replicationBridgeAuthenticationBasicClientUsername">The Client Username the replication Bridge uses to login to the remote Message VPN. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="replicationBridgeAuthenticationBasicPassword">The password for the Client Username. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="replicationBridgeAuthenticationClientCertContent">The PEM formatted content for the client certificate used by this bridge to login to the Remote Message VPN. It must consist of a private key and between one and three certificates comprising the certificate trust chain. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.9..</param>
        /// <param name="replicationBridgeAuthenticationClientCertPassword">The password for the client certificate. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.9..</param>
        /// <param name="replicationBridgeAuthenticationScheme">The authentication scheme for the replication Bridge in the Message VPN. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;basic\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;basic\&quot; - Basic Authentication Scheme (via username and password). \&quot;client-certificate\&quot; - Client Certificate Authentication Scheme (via certificate file or content). &lt;/pre&gt; .</param>
        /// <param name="replicationBridgeCompressedDataEnabled">Enable or disable use of compression for the replication Bridge. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="replicationBridgeEgressFlowWindowSize">The size of the window used for guaranteed messages published to the replication Bridge, in messages. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;255&#x60;..</param>
        /// <param name="replicationBridgeRetryDelay">The number of seconds that must pass before retrying the replication Bridge connection. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;3&#x60;..</param>
        /// <param name="replicationBridgeTlsEnabled">Enable or disable use of encryption (TLS) for the replication Bridge connection. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="replicationBridgeUnidirectionalClientProfileName">The Client Profile for the unidirectional replication Bridge in the Message VPN. It is used only for the TCP parameters. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;#client-profile\&quot;&#x60;..</param>
        /// <param name="replicationEnabled">Enable or disable replication for the Message VPN. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="replicationEnabledQueueBehavior">The behavior to take when enabling replication for the Message VPN, depending on the existence of the replication Queue. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;fail-on-existing-queue\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;fail-on-existing-queue\&quot; - The data replication queue must not already exist. \&quot;force-use-existing-queue\&quot; - The data replication queue must already exist. Any data messages on the Queue will be forwarded to interested applications. IMPORTANT: Before using this mode be certain that the messages are not stale or otherwise unsuitable to be forwarded. This mode can only be specified when the existing queue is configured the same as is currently specified under replication configuration otherwise the enabling of replication will fail. \&quot;force-recreate-queue\&quot; - The data replication queue must already exist. Any data messages on the Queue will be discarded. IMPORTANT: Before using this mode be certain that the messages on the existing data replication queue are not needed by interested applications. &lt;/pre&gt; .</param>
        /// <param name="replicationQueueMaxMsgSpoolUsage">The maximum message spool usage by the replication Bridge local Queue (quota), in megabytes. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;60000&#x60;..</param>
        /// <param name="replicationQueueRejectMsgToSenderOnDiscardEnabled">Enable or disable whether messages discarded on the replication Bridge local Queue are rejected back to the sender. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="replicationRejectMsgWhenSyncIneligibleEnabled">Enable or disable whether guaranteed messages published to synchronously replicated Topics are rejected back to the sender when synchronous replication becomes ineligible. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="replicationRole">The replication role for the Message VPN. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;standby\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;active\&quot; - Assume the Active role in replication for the Message VPN. \&quot;standby\&quot; - Assume the Standby role in replication for the Message VPN. &lt;/pre&gt; .</param>
        /// <param name="replicationTransactionMode">The transaction replication mode for all transactions within the Message VPN. Changing this value during operation will not affect existing transactions; it is only used upon starting a transaction. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;async\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;sync\&quot; - Messages are acknowledged when replicated (spooled remotely). \&quot;async\&quot; - Messages are acknowledged when pending replication (spooled locally). &lt;/pre&gt; .</param>
        /// <param name="restTlsServerCertEnforceTrustedCommonNameEnabled">Enable or disable validation of the Common Name (CN) in the server certificate from the remote REST Consumer. If enabled, the Common Name is checked against the list of Trusted Common Names configured for the REST Consumer. Common Name validation is not performed if Server Certificate Name Validation is enabled, even if Common Name validation is enabled. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.17. Common Name validation has been replaced by Server Certificate Name validation..</param>
        /// <param name="restTlsServerCertMaxChainDepth">The maximum depth for a REST Consumer server certificate chain. The depth of a chain is defined as the number of signing CA certificates that are present in the chain back to a trusted self-signed root CA certificate. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;..</param>
        /// <param name="restTlsServerCertValidateDateEnabled">Enable or disable validation of the \&quot;Not Before\&quot; and \&quot;Not After\&quot; validity dates in the REST Consumer server certificate. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="restTlsServerCertValidateNameEnabled">Enable or disable the standard TLS authentication mechanism of verifying the name used to connect to the remote REST Consumer. If enabled, the name used to connect to the remote REST Consumer is checked against the names specified in the certificate returned by the remote router. Legacy Common Name validation is not performed if Server Certificate Name Validation is enabled, even if Common Name validation is also enabled. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;. Available since 2.17..</param>
        /// <param name="sempOverMsgBusAdminClientEnabled">Enable or disable \&quot;admin client\&quot; SEMP over the message bus commands for the current Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="sempOverMsgBusAdminDistributedCacheEnabled">Enable or disable \&quot;admin distributed-cache\&quot; SEMP over the message bus commands for the current Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="sempOverMsgBusAdminEnabled">Enable or disable \&quot;admin\&quot; SEMP over the message bus commands for the current Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="sempOverMsgBusEnabled">Enable or disable SEMP over the message bus for the current Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="sempOverMsgBusShowEnabled">Enable or disable \&quot;show\&quot; SEMP over the message bus commands for the current Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="serviceAmqpMaxConnectionCount">The maximum number of AMQP client connections that can be simultaneously connected to the Message VPN. This value may be higher than supported by the platform. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform. Available since 2.7..</param>
        /// <param name="serviceAmqpPlainTextEnabled">Enable or disable the plain-text AMQP service in the Message VPN. Disabling causes clients connected to the corresponding listen-port to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.7..</param>
        /// <param name="serviceAmqpPlainTextListenPort">The port number for plain-text AMQP clients that connect to the Message VPN. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceAmqpPlainTextEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.7..</param>
        /// <param name="serviceAmqpTlsEnabled">Enable or disable the use of encryption (TLS) for the AMQP service in the Message VPN. Disabling causes clients currently connected over TLS to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.7..</param>
        /// <param name="serviceAmqpTlsListenPort">The port number for AMQP clients that connect to the Message VPN over TLS. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceAmqpTlsEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.7..</param>
        /// <param name="serviceMqttAuthenticationClientCertRequest">Determines when to request a client certificate from an incoming MQTT client connecting via a TLS port. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-enabled-in-message-vpn\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;never\&quot; - Never ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;when-enabled-in-message-vpn\&quot; - Only ask for a client-certificate if client certificate authentication is enabled under \&quot;message-vpn &gt;  authentication &gt; client-certificate &gt; shutdown\&quot;. &lt;/pre&gt;  Available since 2.21..</param>
        /// <param name="serviceMqttMaxConnectionCount">The maximum number of MQTT client connections that can be simultaneously connected to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform. Available since 2.1..</param>
        /// <param name="serviceMqttPlainTextEnabled">Enable or disable the plain-text MQTT service in the Message VPN. Disabling causes clients currently connected to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.1..</param>
        /// <param name="serviceMqttPlainTextListenPort">The port number for plain-text MQTT clients that connect to the Message VPN. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceMqttPlainTextEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.1..</param>
        /// <param name="serviceMqttTlsEnabled">Enable or disable the use of encryption (TLS) for the MQTT service in the Message VPN. Disabling causes clients currently connected over TLS to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.1..</param>
        /// <param name="serviceMqttTlsListenPort">The port number for MQTT clients that connect to the Message VPN over TLS. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceMqttTlsEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.1..</param>
        /// <param name="serviceMqttTlsWebSocketEnabled">Enable or disable the use of encrypted WebSocket (WebSocket over TLS) for the MQTT service in the Message VPN. Disabling causes clients currently connected by encrypted WebSocket to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.1..</param>
        /// <param name="serviceMqttTlsWebSocketListenPort">The port number for MQTT clients that connect to the Message VPN using WebSocket over TLS. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceMqttTlsWebSocketEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.1..</param>
        /// <param name="serviceMqttWebSocketEnabled">Enable or disable the use of WebSocket for the MQTT service in the Message VPN. Disabling causes clients currently connected by WebSocket to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.1..</param>
        /// <param name="serviceMqttWebSocketListenPort">The port number for plain-text MQTT clients that connect to the Message VPN using WebSocket. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceMqttWebSocketEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.1..</param>
        /// <param name="serviceRestIncomingAuthenticationClientCertRequest">Determines when to request a client certificate from an incoming REST Producer connecting via a TLS port. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-enabled-in-message-vpn\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;never\&quot; - Never ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;when-enabled-in-message-vpn\&quot; - Only ask for a client-certificate if client certificate authentication is enabled under \&quot;message-vpn &gt;  authentication &gt; client-certificate &gt; shutdown\&quot;. &lt;/pre&gt;  Available since 2.21..</param>
        /// <param name="serviceRestIncomingAuthorizationHeaderHandling">The handling of Authorization headers for incoming REST connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;drop\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;drop\&quot; - Do not attach the Authorization header to the message as a user property. This configuration is most secure. \&quot;forward\&quot; - Forward the Authorization header, attaching it to the message as a user property in the same way as other headers. For best security, use the drop setting. \&quot;legacy\&quot; - If the Authorization header was used for authentication to the broker, do not attach it to the message. If the Authorization header was not used for authentication to the broker, attach it to the message as a user property in the same way as other headers. For best security, use the drop setting. &lt;/pre&gt;  Available since 2.19..</param>
        /// <param name="serviceRestIncomingMaxConnectionCount">The maximum number of REST incoming client connections that can be simultaneously connected to the Message VPN. This value may be higher than supported by the platform. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform..</param>
        /// <param name="serviceRestIncomingPlainTextEnabled">Enable or disable the plain-text REST service for incoming clients in the Message VPN. Disabling causes clients currently connected to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="serviceRestIncomingPlainTextListenPort">The port number for incoming plain-text REST clients that connect to the Message VPN. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceRestIncomingPlainTextEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;..</param>
        /// <param name="serviceRestIncomingTlsEnabled">Enable or disable the use of encryption (TLS) for the REST service for incoming clients in the Message VPN. Disabling causes clients currently connected over TLS to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="serviceRestIncomingTlsListenPort">The port number for incoming REST clients that connect to the Message VPN over TLS. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceRestIncomingTlsEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;..</param>
        /// <param name="serviceRestMode">The REST service mode for incoming REST clients that connect to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;messaging\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;gateway\&quot; - Act as a message gateway through which REST messages are propagated. \&quot;messaging\&quot; - Act as a message broker on which REST messages are queued. &lt;/pre&gt;  Available since 2.6..</param>
        /// <param name="serviceRestOutgoingMaxConnectionCount">The maximum number of REST Consumer (outgoing) client connections that can be simultaneously connected to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default varies by platform..</param>
        /// <param name="serviceSmfMaxConnectionCount">The maximum number of SMF client connections that can be simultaneously connected to the Message VPN. This value may be higher than supported by the platform. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default varies by platform..</param>
        /// <param name="serviceSmfPlainTextEnabled">Enable or disable the plain-text SMF service in the Message VPN. Disabling causes clients currently connected to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="serviceSmfTlsEnabled">Enable or disable the use of encryption (TLS) for the SMF service in the Message VPN. Disabling causes clients currently connected over TLS to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="serviceWebAuthenticationClientCertRequest">Determines when to request a client certificate from a Web Transport client connecting via a TLS port. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;when-enabled-in-message-vpn\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;never\&quot; - Never ask for a client certificate regardless of the \&quot;message-vpn &gt; authentication &gt; client-certificate &gt; shutdown\&quot; configuration. \&quot;when-enabled-in-message-vpn\&quot; - Only ask for a client-certificate if client certificate authentication is enabled under \&quot;message-vpn &gt;  authentication &gt; client-certificate &gt; shutdown\&quot;. &lt;/pre&gt;  Available since 2.21..</param>
        /// <param name="serviceWebMaxConnectionCount">The maximum number of Web Transport client connections that can be simultaneously connected to the Message VPN. This value may be higher than supported by the platform. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform..</param>
        /// <param name="serviceWebPlainTextEnabled">Enable or disable the plain-text Web Transport service in the Message VPN. Disabling causes clients currently connected to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="serviceWebTlsEnabled">Enable or disable the use of TLS for the Web Transport service in the Message VPN. Disabling causes clients currently connected over TLS to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="tlsAllowDowngradeToPlainTextEnabled">Enable or disable the allowing of TLS SMF clients to downgrade their connections to plain-text connections. Changing this will not affect existing connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        public MsgVpn(string alias = default(string), bool? authenticationBasicEnabled = default(bool?), string authenticationBasicProfileName = default(string), string authenticationBasicRadiusDomain = default(string), AuthenticationBasicTypeEnum? authenticationBasicType = default(AuthenticationBasicTypeEnum?), bool? authenticationClientCertAllowApiProvidedUsernameEnabled = default(bool?), bool? authenticationClientCertCertificateMatchingRulesEnabled = default(bool?), bool? authenticationClientCertEnabled = default(bool?), long? authenticationClientCertMaxChainDepth = default(long?), AuthenticationClientCertRevocationCheckModeEnum? authenticationClientCertRevocationCheckMode = default(AuthenticationClientCertRevocationCheckModeEnum?), AuthenticationClientCertUsernameSourceEnum? authenticationClientCertUsernameSource = default(AuthenticationClientCertUsernameSourceEnum?), bool? authenticationClientCertValidateDateEnabled = default(bool?), bool? authenticationKerberosAllowApiProvidedUsernameEnabled = default(bool?), bool? authenticationKerberosEnabled = default(bool?), string authenticationOauthDefaultProfileName = default(string), string authenticationOauthDefaultProviderName = default(string), bool? authenticationOauthEnabled = default(bool?), string authorizationLdapGroupMembershipAttributeName = default(string), bool? authorizationLdapTrimClientUsernameDomainEnabled = default(bool?), string authorizationProfileName = default(string), AuthorizationTypeEnum? authorizationType = default(AuthorizationTypeEnum?), bool? bridgingTlsServerCertEnforceTrustedCommonNameEnabled = default(bool?), long? bridgingTlsServerCertMaxChainDepth = default(long?), bool? bridgingTlsServerCertValidateDateEnabled = default(bool?), bool? bridgingTlsServerCertValidateNameEnabled = default(bool?), bool? distributedCacheManagementEnabled = default(bool?), bool? dmrEnabled = default(bool?), bool? enabled = default(bool?), EventThreshold eventConnectionCountThreshold = default(EventThreshold), EventThreshold eventEgressFlowCountThreshold = default(EventThreshold), EventThresholdByValue eventEgressMsgRateThreshold = default(EventThresholdByValue), EventThreshold eventEndpointCountThreshold = default(EventThreshold), EventThreshold eventIngressFlowCountThreshold = default(EventThreshold), EventThresholdByValue eventIngressMsgRateThreshold = default(EventThresholdByValue), long? eventLargeMsgThreshold = default(long?), string eventLogTag = default(string), EventThreshold eventMsgSpoolUsageThreshold = default(EventThreshold), bool? eventPublishClientEnabled = default(bool?), bool? eventPublishMsgVpnEnabled = default(bool?), EventPublishSubscriptionModeEnum? eventPublishSubscriptionMode = default(EventPublishSubscriptionModeEnum?), bool? eventPublishTopicFormatMqttEnabled = default(bool?), bool? eventPublishTopicFormatSmfEnabled = default(bool?), EventThreshold eventServiceAmqpConnectionCountThreshold = default(EventThreshold), EventThreshold eventServiceMqttConnectionCountThreshold = default(EventThreshold), EventThreshold eventServiceRestIncomingConnectionCountThreshold = default(EventThreshold), EventThreshold eventServiceSmfConnectionCountThreshold = default(EventThreshold), EventThreshold eventServiceWebConnectionCountThreshold = default(EventThreshold), EventThreshold eventSubscriptionCountThreshold = default(EventThreshold), EventThreshold eventTransactedSessionCountThreshold = default(EventThreshold), EventThreshold eventTransactionCountThreshold = default(EventThreshold), bool? exportSubscriptionsEnabled = default(bool?), bool? jndiEnabled = default(bool?), long? maxConnectionCount = default(long?), long? maxEgressFlowCount = default(long?), long? maxEndpointCount = default(long?), long? maxIngressFlowCount = default(long?), long? maxMsgSpoolUsage = default(long?), long? maxSubscriptionCount = default(long?), long? maxTransactedSessionCount = default(long?), long? maxTransactionCount = default(long?), int? mqttRetainMaxMemory = default(int?), string msgVpnName = default(string), long? replicationAckPropagationIntervalMsgCount = default(long?), string replicationBridgeAuthenticationBasicClientUsername = default(string), string replicationBridgeAuthenticationBasicPassword = default(string), string replicationBridgeAuthenticationClientCertContent = default(string), string replicationBridgeAuthenticationClientCertPassword = default(string), ReplicationBridgeAuthenticationSchemeEnum? replicationBridgeAuthenticationScheme = default(ReplicationBridgeAuthenticationSchemeEnum?), bool? replicationBridgeCompressedDataEnabled = default(bool?), long? replicationBridgeEgressFlowWindowSize = default(long?), long? replicationBridgeRetryDelay = default(long?), bool? replicationBridgeTlsEnabled = default(bool?), string replicationBridgeUnidirectionalClientProfileName = default(string), bool? replicationEnabled = default(bool?), ReplicationEnabledQueueBehaviorEnum? replicationEnabledQueueBehavior = default(ReplicationEnabledQueueBehaviorEnum?), long? replicationQueueMaxMsgSpoolUsage = default(long?), bool? replicationQueueRejectMsgToSenderOnDiscardEnabled = default(bool?), bool? replicationRejectMsgWhenSyncIneligibleEnabled = default(bool?), ReplicationRoleEnum? replicationRole = default(ReplicationRoleEnum?), ReplicationTransactionModeEnum? replicationTransactionMode = default(ReplicationTransactionModeEnum?), bool? restTlsServerCertEnforceTrustedCommonNameEnabled = default(bool?), long? restTlsServerCertMaxChainDepth = default(long?), bool? restTlsServerCertValidateDateEnabled = default(bool?), bool? restTlsServerCertValidateNameEnabled = default(bool?), bool? sempOverMsgBusAdminClientEnabled = default(bool?), bool? sempOverMsgBusAdminDistributedCacheEnabled = default(bool?), bool? sempOverMsgBusAdminEnabled = default(bool?), bool? sempOverMsgBusEnabled = default(bool?), bool? sempOverMsgBusShowEnabled = default(bool?), long? serviceAmqpMaxConnectionCount = default(long?), bool? serviceAmqpPlainTextEnabled = default(bool?), long? serviceAmqpPlainTextListenPort = default(long?), bool? serviceAmqpTlsEnabled = default(bool?), long? serviceAmqpTlsListenPort = default(long?), ServiceMqttAuthenticationClientCertRequestEnum? serviceMqttAuthenticationClientCertRequest = default(ServiceMqttAuthenticationClientCertRequestEnum?), long? serviceMqttMaxConnectionCount = default(long?), bool? serviceMqttPlainTextEnabled = default(bool?), long? serviceMqttPlainTextListenPort = default(long?), bool? serviceMqttTlsEnabled = default(bool?), long? serviceMqttTlsListenPort = default(long?), bool? serviceMqttTlsWebSocketEnabled = default(bool?), long? serviceMqttTlsWebSocketListenPort = default(long?), bool? serviceMqttWebSocketEnabled = default(bool?), long? serviceMqttWebSocketListenPort = default(long?), ServiceRestIncomingAuthenticationClientCertRequestEnum? serviceRestIncomingAuthenticationClientCertRequest = default(ServiceRestIncomingAuthenticationClientCertRequestEnum?), ServiceRestIncomingAuthorizationHeaderHandlingEnum? serviceRestIncomingAuthorizationHeaderHandling = default(ServiceRestIncomingAuthorizationHeaderHandlingEnum?), long? serviceRestIncomingMaxConnectionCount = default(long?), bool? serviceRestIncomingPlainTextEnabled = default(bool?), long? serviceRestIncomingPlainTextListenPort = default(long?), bool? serviceRestIncomingTlsEnabled = default(bool?), long? serviceRestIncomingTlsListenPort = default(long?), ServiceRestModeEnum? serviceRestMode = default(ServiceRestModeEnum?), long? serviceRestOutgoingMaxConnectionCount = default(long?), long? serviceSmfMaxConnectionCount = default(long?), bool? serviceSmfPlainTextEnabled = default(bool?), bool? serviceSmfTlsEnabled = default(bool?), ServiceWebAuthenticationClientCertRequestEnum? serviceWebAuthenticationClientCertRequest = default(ServiceWebAuthenticationClientCertRequestEnum?), long? serviceWebMaxConnectionCount = default(long?), bool? serviceWebPlainTextEnabled = default(bool?), bool? serviceWebTlsEnabled = default(bool?), bool? tlsAllowDowngradeToPlainTextEnabled = default(bool?))
        {
            this.Alias = alias;
            this.AuthenticationBasicEnabled = authenticationBasicEnabled;
            this.AuthenticationBasicProfileName = authenticationBasicProfileName;
            this.AuthenticationBasicRadiusDomain = authenticationBasicRadiusDomain;
            this.AuthenticationBasicType = authenticationBasicType;
            this.AuthenticationClientCertAllowApiProvidedUsernameEnabled = authenticationClientCertAllowApiProvidedUsernameEnabled;
            this.AuthenticationClientCertCertificateMatchingRulesEnabled = authenticationClientCertCertificateMatchingRulesEnabled;
            this.AuthenticationClientCertEnabled = authenticationClientCertEnabled;
            this.AuthenticationClientCertMaxChainDepth = authenticationClientCertMaxChainDepth;
            this.AuthenticationClientCertRevocationCheckMode = authenticationClientCertRevocationCheckMode;
            this.AuthenticationClientCertUsernameSource = authenticationClientCertUsernameSource;
            this.AuthenticationClientCertValidateDateEnabled = authenticationClientCertValidateDateEnabled;
            this.AuthenticationKerberosAllowApiProvidedUsernameEnabled = authenticationKerberosAllowApiProvidedUsernameEnabled;
            this.AuthenticationKerberosEnabled = authenticationKerberosEnabled;
            this.AuthenticationOauthDefaultProfileName = authenticationOauthDefaultProfileName;
            this.AuthenticationOauthDefaultProviderName = authenticationOauthDefaultProviderName;
            this.AuthenticationOauthEnabled = authenticationOauthEnabled;
            this.AuthorizationLdapGroupMembershipAttributeName = authorizationLdapGroupMembershipAttributeName;
            this.AuthorizationLdapTrimClientUsernameDomainEnabled = authorizationLdapTrimClientUsernameDomainEnabled;
            this.AuthorizationProfileName = authorizationProfileName;
            this.AuthorizationType = authorizationType;
            this.BridgingTlsServerCertEnforceTrustedCommonNameEnabled = bridgingTlsServerCertEnforceTrustedCommonNameEnabled;
            this.BridgingTlsServerCertMaxChainDepth = bridgingTlsServerCertMaxChainDepth;
            this.BridgingTlsServerCertValidateDateEnabled = bridgingTlsServerCertValidateDateEnabled;
            this.BridgingTlsServerCertValidateNameEnabled = bridgingTlsServerCertValidateNameEnabled;
            this.DistributedCacheManagementEnabled = distributedCacheManagementEnabled;
            this.DmrEnabled = dmrEnabled;
            this.Enabled = enabled;
            this.EventConnectionCountThreshold = eventConnectionCountThreshold;
            this.EventEgressFlowCountThreshold = eventEgressFlowCountThreshold;
            this.EventEgressMsgRateThreshold = eventEgressMsgRateThreshold;
            this.EventEndpointCountThreshold = eventEndpointCountThreshold;
            this.EventIngressFlowCountThreshold = eventIngressFlowCountThreshold;
            this.EventIngressMsgRateThreshold = eventIngressMsgRateThreshold;
            this.EventLargeMsgThreshold = eventLargeMsgThreshold;
            this.EventLogTag = eventLogTag;
            this.EventMsgSpoolUsageThreshold = eventMsgSpoolUsageThreshold;
            this.EventPublishClientEnabled = eventPublishClientEnabled;
            this.EventPublishMsgVpnEnabled = eventPublishMsgVpnEnabled;
            this.EventPublishSubscriptionMode = eventPublishSubscriptionMode;
            this.EventPublishTopicFormatMqttEnabled = eventPublishTopicFormatMqttEnabled;
            this.EventPublishTopicFormatSmfEnabled = eventPublishTopicFormatSmfEnabled;
            this.EventServiceAmqpConnectionCountThreshold = eventServiceAmqpConnectionCountThreshold;
            this.EventServiceMqttConnectionCountThreshold = eventServiceMqttConnectionCountThreshold;
            this.EventServiceRestIncomingConnectionCountThreshold = eventServiceRestIncomingConnectionCountThreshold;
            this.EventServiceSmfConnectionCountThreshold = eventServiceSmfConnectionCountThreshold;
            this.EventServiceWebConnectionCountThreshold = eventServiceWebConnectionCountThreshold;
            this.EventSubscriptionCountThreshold = eventSubscriptionCountThreshold;
            this.EventTransactedSessionCountThreshold = eventTransactedSessionCountThreshold;
            this.EventTransactionCountThreshold = eventTransactionCountThreshold;
            this.ExportSubscriptionsEnabled = exportSubscriptionsEnabled;
            this.JndiEnabled = jndiEnabled;
            this.MaxConnectionCount = maxConnectionCount;
            this.MaxEgressFlowCount = maxEgressFlowCount;
            this.MaxEndpointCount = maxEndpointCount;
            this.MaxIngressFlowCount = maxIngressFlowCount;
            this.MaxMsgSpoolUsage = maxMsgSpoolUsage;
            this.MaxSubscriptionCount = maxSubscriptionCount;
            this.MaxTransactedSessionCount = maxTransactedSessionCount;
            this.MaxTransactionCount = maxTransactionCount;
            this.MqttRetainMaxMemory = mqttRetainMaxMemory;
            this.MsgVpnName = msgVpnName;
            this.ReplicationAckPropagationIntervalMsgCount = replicationAckPropagationIntervalMsgCount;
            this.ReplicationBridgeAuthenticationBasicClientUsername = replicationBridgeAuthenticationBasicClientUsername;
            this.ReplicationBridgeAuthenticationBasicPassword = replicationBridgeAuthenticationBasicPassword;
            this.ReplicationBridgeAuthenticationClientCertContent = replicationBridgeAuthenticationClientCertContent;
            this.ReplicationBridgeAuthenticationClientCertPassword = replicationBridgeAuthenticationClientCertPassword;
            this.ReplicationBridgeAuthenticationScheme = replicationBridgeAuthenticationScheme;
            this.ReplicationBridgeCompressedDataEnabled = replicationBridgeCompressedDataEnabled;
            this.ReplicationBridgeEgressFlowWindowSize = replicationBridgeEgressFlowWindowSize;
            this.ReplicationBridgeRetryDelay = replicationBridgeRetryDelay;
            this.ReplicationBridgeTlsEnabled = replicationBridgeTlsEnabled;
            this.ReplicationBridgeUnidirectionalClientProfileName = replicationBridgeUnidirectionalClientProfileName;
            this.ReplicationEnabled = replicationEnabled;
            this.ReplicationEnabledQueueBehavior = replicationEnabledQueueBehavior;
            this.ReplicationQueueMaxMsgSpoolUsage = replicationQueueMaxMsgSpoolUsage;
            this.ReplicationQueueRejectMsgToSenderOnDiscardEnabled = replicationQueueRejectMsgToSenderOnDiscardEnabled;
            this.ReplicationRejectMsgWhenSyncIneligibleEnabled = replicationRejectMsgWhenSyncIneligibleEnabled;
            this.ReplicationRole = replicationRole;
            this.ReplicationTransactionMode = replicationTransactionMode;
            this.RestTlsServerCertEnforceTrustedCommonNameEnabled = restTlsServerCertEnforceTrustedCommonNameEnabled;
            this.RestTlsServerCertMaxChainDepth = restTlsServerCertMaxChainDepth;
            this.RestTlsServerCertValidateDateEnabled = restTlsServerCertValidateDateEnabled;
            this.RestTlsServerCertValidateNameEnabled = restTlsServerCertValidateNameEnabled;
            this.SempOverMsgBusAdminClientEnabled = sempOverMsgBusAdminClientEnabled;
            this.SempOverMsgBusAdminDistributedCacheEnabled = sempOverMsgBusAdminDistributedCacheEnabled;
            this.SempOverMsgBusAdminEnabled = sempOverMsgBusAdminEnabled;
            this.SempOverMsgBusEnabled = sempOverMsgBusEnabled;
            this.SempOverMsgBusShowEnabled = sempOverMsgBusShowEnabled;
            this.ServiceAmqpMaxConnectionCount = serviceAmqpMaxConnectionCount;
            this.ServiceAmqpPlainTextEnabled = serviceAmqpPlainTextEnabled;
            this.ServiceAmqpPlainTextListenPort = serviceAmqpPlainTextListenPort;
            this.ServiceAmqpTlsEnabled = serviceAmqpTlsEnabled;
            this.ServiceAmqpTlsListenPort = serviceAmqpTlsListenPort;
            this.ServiceMqttAuthenticationClientCertRequest = serviceMqttAuthenticationClientCertRequest;
            this.ServiceMqttMaxConnectionCount = serviceMqttMaxConnectionCount;
            this.ServiceMqttPlainTextEnabled = serviceMqttPlainTextEnabled;
            this.ServiceMqttPlainTextListenPort = serviceMqttPlainTextListenPort;
            this.ServiceMqttTlsEnabled = serviceMqttTlsEnabled;
            this.ServiceMqttTlsListenPort = serviceMqttTlsListenPort;
            this.ServiceMqttTlsWebSocketEnabled = serviceMqttTlsWebSocketEnabled;
            this.ServiceMqttTlsWebSocketListenPort = serviceMqttTlsWebSocketListenPort;
            this.ServiceMqttWebSocketEnabled = serviceMqttWebSocketEnabled;
            this.ServiceMqttWebSocketListenPort = serviceMqttWebSocketListenPort;
            this.ServiceRestIncomingAuthenticationClientCertRequest = serviceRestIncomingAuthenticationClientCertRequest;
            this.ServiceRestIncomingAuthorizationHeaderHandling = serviceRestIncomingAuthorizationHeaderHandling;
            this.ServiceRestIncomingMaxConnectionCount = serviceRestIncomingMaxConnectionCount;
            this.ServiceRestIncomingPlainTextEnabled = serviceRestIncomingPlainTextEnabled;
            this.ServiceRestIncomingPlainTextListenPort = serviceRestIncomingPlainTextListenPort;
            this.ServiceRestIncomingTlsEnabled = serviceRestIncomingTlsEnabled;
            this.ServiceRestIncomingTlsListenPort = serviceRestIncomingTlsListenPort;
            this.ServiceRestMode = serviceRestMode;
            this.ServiceRestOutgoingMaxConnectionCount = serviceRestOutgoingMaxConnectionCount;
            this.ServiceSmfMaxConnectionCount = serviceSmfMaxConnectionCount;
            this.ServiceSmfPlainTextEnabled = serviceSmfPlainTextEnabled;
            this.ServiceSmfTlsEnabled = serviceSmfTlsEnabled;
            this.ServiceWebAuthenticationClientCertRequest = serviceWebAuthenticationClientCertRequest;
            this.ServiceWebMaxConnectionCount = serviceWebMaxConnectionCount;
            this.ServiceWebPlainTextEnabled = serviceWebPlainTextEnabled;
            this.ServiceWebTlsEnabled = serviceWebTlsEnabled;
            this.TlsAllowDowngradeToPlainTextEnabled = tlsAllowDowngradeToPlainTextEnabled;
        }
        
        /// <summary>
        /// The name of another Message VPN which this Message VPN is an alias for. When this Message VPN is enabled, the alias has no effect. When this Message VPN is disabled, Clients (but not Bridges and routing Links) logging into this Message VPN are automatically logged in to the other Message VPN, and authentication and authorization take place in the context of the other Message VPN.  Aliases may form a non-circular chain, cascading one to the next. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.14.
        /// </summary>
        /// <value>The name of another Message VPN which this Message VPN is an alias for. When this Message VPN is enabled, the alias has no effect. When this Message VPN is disabled, Clients (but not Bridges and routing Links) logging into this Message VPN are automatically logged in to the other Message VPN, and authentication and authorization take place in the context of the other Message VPN.  Aliases may form a non-circular chain, cascading one to the next. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.14.</value>
        [DataMember(Name="alias", EmitDefaultValue=false)]
        public string Alias { get; set; }

        /// <summary>
        /// Enable or disable basic authentication for clients connecting to the Message VPN. Basic authentication is authentication that involves the use of a username and password to prove identity. If a user provides credentials for a different authentication scheme, this setting is not applicable. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable basic authentication for clients connecting to the Message VPN. Basic authentication is authentication that involves the use of a username and password to prove identity. If a user provides credentials for a different authentication scheme, this setting is not applicable. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="authenticationBasicEnabled", EmitDefaultValue=false)]
        public bool? AuthenticationBasicEnabled { get; set; }

        /// <summary>
        /// The name of the RADIUS or LDAP Profile to use for basic authentication. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;.
        /// </summary>
        /// <value>The name of the RADIUS or LDAP Profile to use for basic authentication. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;.</value>
        [DataMember(Name="authenticationBasicProfileName", EmitDefaultValue=false)]
        public string AuthenticationBasicProfileName { get; set; }

        /// <summary>
        /// The RADIUS domain to use for basic authentication. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The RADIUS domain to use for basic authentication. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="authenticationBasicRadiusDomain", EmitDefaultValue=false)]
        public string AuthenticationBasicRadiusDomain { get; set; }


        /// <summary>
        /// Enable or disable allowing a client to specify a Client Username via the API connect method. When disabled, the certificate CN (Common Name) is always used. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable allowing a client to specify a Client Username via the API connect method. When disabled, the certificate CN (Common Name) is always used. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="authenticationClientCertAllowApiProvidedUsernameEnabled", EmitDefaultValue=false)]
        public bool? AuthenticationClientCertAllowApiProvidedUsernameEnabled { get; set; }

        /// <summary>
        /// Enable or disable certificate matching rules. When disabled, any valid certificate is accepted. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.27.
        /// </summary>
        /// <value>Enable or disable certificate matching rules. When disabled, any valid certificate is accepted. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.27.</value>
        [DataMember(Name="authenticationClientCertCertificateMatchingRulesEnabled", EmitDefaultValue=false)]
        public bool? AuthenticationClientCertCertificateMatchingRulesEnabled { get; set; }

        /// <summary>
        /// Enable or disable client certificate authentication in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable client certificate authentication in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="authenticationClientCertEnabled", EmitDefaultValue=false)]
        public bool? AuthenticationClientCertEnabled { get; set; }

        /// <summary>
        /// The maximum depth for a client certificate chain. The depth of a chain is defined as the number of signing CA certificates that are present in the chain back to a trusted self-signed root CA certificate. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;.
        /// </summary>
        /// <value>The maximum depth for a client certificate chain. The depth of a chain is defined as the number of signing CA certificates that are present in the chain back to a trusted self-signed root CA certificate. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;.</value>
        [DataMember(Name="authenticationClientCertMaxChainDepth", EmitDefaultValue=false)]
        public long? AuthenticationClientCertMaxChainDepth { get; set; }



        /// <summary>
        /// Enable or disable validation of the \&quot;Not Before\&quot; and \&quot;Not After\&quot; validity dates in the client certificate. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable validation of the \&quot;Not Before\&quot; and \&quot;Not After\&quot; validity dates in the client certificate. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="authenticationClientCertValidateDateEnabled", EmitDefaultValue=false)]
        public bool? AuthenticationClientCertValidateDateEnabled { get; set; }

        /// <summary>
        /// Enable or disable allowing a client to specify a Client Username via the API connect method. When disabled, the Kerberos Principal name is always used. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable allowing a client to specify a Client Username via the API connect method. When disabled, the Kerberos Principal name is always used. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="authenticationKerberosAllowApiProvidedUsernameEnabled", EmitDefaultValue=false)]
        public bool? AuthenticationKerberosAllowApiProvidedUsernameEnabled { get; set; }

        /// <summary>
        /// Enable or disable Kerberos authentication in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable Kerberos authentication in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="authenticationKerberosEnabled", EmitDefaultValue=false)]
        public bool? AuthenticationKerberosEnabled { get; set; }

        /// <summary>
        /// The name of the profile to use when the client does not supply a profile name. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.25.
        /// </summary>
        /// <value>The name of the profile to use when the client does not supply a profile name. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.25.</value>
        [DataMember(Name="authenticationOauthDefaultProfileName", EmitDefaultValue=false)]
        public string AuthenticationOauthDefaultProfileName { get; set; }

        /// <summary>
        /// The name of the provider to use when the client does not supply a provider name. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.25. authenticationOauthDefaultProviderName and authenticationOauthProviders replaced by authenticationOauthDefaultProfileName and authenticationOauthProfiles.
        /// </summary>
        /// <value>The name of the provider to use when the client does not supply a provider name. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Deprecated since 2.25. authenticationOauthDefaultProviderName and authenticationOauthProviders replaced by authenticationOauthDefaultProfileName and authenticationOauthProfiles.</value>
        [DataMember(Name="authenticationOauthDefaultProviderName", EmitDefaultValue=false)]
        public string AuthenticationOauthDefaultProviderName { get; set; }

        /// <summary>
        /// Enable or disable OAuth authentication. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.13.
        /// </summary>
        /// <value>Enable or disable OAuth authentication. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.13.</value>
        [DataMember(Name="authenticationOauthEnabled", EmitDefaultValue=false)]
        public bool? AuthenticationOauthEnabled { get; set; }

        /// <summary>
        /// The name of the attribute that is retrieved from the LDAP server as part of the LDAP search when authorizing a client connecting to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;memberOf\&quot;&#x60;.
        /// </summary>
        /// <value>The name of the attribute that is retrieved from the LDAP server as part of the LDAP search when authorizing a client connecting to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;memberOf\&quot;&#x60;.</value>
        [DataMember(Name="authorizationLdapGroupMembershipAttributeName", EmitDefaultValue=false)]
        public string AuthorizationLdapGroupMembershipAttributeName { get; set; }

        /// <summary>
        /// Enable or disable client-username domain trimming for LDAP lookups of client connections. When enabled, the value of $CLIENT_USERNAME (when used for searching) will be truncated at the first occurance of the @ character. For example, if the client-username is in the form of an email address, then the domain portion will be removed. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.13.
        /// </summary>
        /// <value>Enable or disable client-username domain trimming for LDAP lookups of client connections. When enabled, the value of $CLIENT_USERNAME (when used for searching) will be truncated at the first occurance of the @ character. For example, if the client-username is in the form of an email address, then the domain portion will be removed. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.13.</value>
        [DataMember(Name="authorizationLdapTrimClientUsernameDomainEnabled", EmitDefaultValue=false)]
        public bool? AuthorizationLdapTrimClientUsernameDomainEnabled { get; set; }

        /// <summary>
        /// The name of the LDAP Profile to use for client authorization. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The name of the LDAP Profile to use for client authorization. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="authorizationProfileName", EmitDefaultValue=false)]
        public string AuthorizationProfileName { get; set; }


        /// <summary>
        /// Enable or disable validation of the Common Name (CN) in the server certificate from the remote broker. If enabled, the Common Name is checked against the list of Trusted Common Names configured for the Bridge. Common Name validation is not performed if Server Certificate Name Validation is enabled, even if Common Name validation is enabled. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <value>Enable or disable validation of the Common Name (CN) in the server certificate from the remote broker. If enabled, the Common Name is checked against the list of Trusted Common Names configured for the Bridge. Common Name validation is not performed if Server Certificate Name Validation is enabled, even if Common Name validation is enabled. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.</value>
        [DataMember(Name="bridgingTlsServerCertEnforceTrustedCommonNameEnabled", EmitDefaultValue=false)]
        public bool? BridgingTlsServerCertEnforceTrustedCommonNameEnabled { get; set; }

        /// <summary>
        /// The maximum depth for a server certificate chain. The depth of a chain is defined as the number of signing CA certificates that are present in the chain back to a trusted self-signed root CA certificate. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;.
        /// </summary>
        /// <value>The maximum depth for a server certificate chain. The depth of a chain is defined as the number of signing CA certificates that are present in the chain back to a trusted self-signed root CA certificate. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;.</value>
        [DataMember(Name="bridgingTlsServerCertMaxChainDepth", EmitDefaultValue=false)]
        public long? BridgingTlsServerCertMaxChainDepth { get; set; }

        /// <summary>
        /// Enable or disable validation of the \&quot;Not Before\&quot; and \&quot;Not After\&quot; validity dates in the server certificate. When disabled, a certificate will be accepted even if the certificate is not valid based on these dates. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable validation of the \&quot;Not Before\&quot; and \&quot;Not After\&quot; validity dates in the server certificate. When disabled, a certificate will be accepted even if the certificate is not valid based on these dates. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="bridgingTlsServerCertValidateDateEnabled", EmitDefaultValue=false)]
        public bool? BridgingTlsServerCertValidateDateEnabled { get; set; }

        /// <summary>
        /// Enable or disable the standard TLS authentication mechanism of verifying the name used to connect to the bridge. If enabled, the name used to connect to the bridge is checked against the names specified in the certificate returned by the remote router. Legacy Common Name validation is not performed if Server Certificate Name Validation is enabled, even if Common Name validation is also enabled. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;. Available since 2.18.
        /// </summary>
        /// <value>Enable or disable the standard TLS authentication mechanism of verifying the name used to connect to the bridge. If enabled, the name used to connect to the bridge is checked against the names specified in the certificate returned by the remote router. Legacy Common Name validation is not performed if Server Certificate Name Validation is enabled, even if Common Name validation is also enabled. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;. Available since 2.18.</value>
        [DataMember(Name="bridgingTlsServerCertValidateNameEnabled", EmitDefaultValue=false)]
        public bool? BridgingTlsServerCertValidateNameEnabled { get; set; }

        /// <summary>
        /// Enable or disable managing of cache instances over the message bus. The default value is &#x60;true&#x60;. Deprecated since 2.28. Distributed cache mangement is now redundancy aware and thus no longer requires administrative intervention for operational state.
        /// </summary>
        /// <value>Enable or disable managing of cache instances over the message bus. The default value is &#x60;true&#x60;. Deprecated since 2.28. Distributed cache mangement is now redundancy aware and thus no longer requires administrative intervention for operational state.</value>
        [DataMember(Name="distributedCacheManagementEnabled", EmitDefaultValue=false)]
        public bool? DistributedCacheManagementEnabled { get; set; }

        /// <summary>
        /// Enable or disable Dynamic Message Routing (DMR) for the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.11.
        /// </summary>
        /// <value>Enable or disable Dynamic Message Routing (DMR) for the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.11.</value>
        [DataMember(Name="dmrEnabled", EmitDefaultValue=false)]
        public bool? DmrEnabled { get; set; }

        /// <summary>
        /// Enable or disable the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="enabled", EmitDefaultValue=false)]
        public bool? Enabled { get; set; }

        /// <summary>
        /// Gets or Sets EventConnectionCountThreshold
        /// </summary>
        [DataMember(Name="eventConnectionCountThreshold", EmitDefaultValue=false)]
        public EventThreshold EventConnectionCountThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventEgressFlowCountThreshold
        /// </summary>
        [DataMember(Name="eventEgressFlowCountThreshold", EmitDefaultValue=false)]
        public EventThreshold EventEgressFlowCountThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventEgressMsgRateThreshold
        /// </summary>
        [DataMember(Name="eventEgressMsgRateThreshold", EmitDefaultValue=false)]
        public EventThresholdByValue EventEgressMsgRateThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventEndpointCountThreshold
        /// </summary>
        [DataMember(Name="eventEndpointCountThreshold", EmitDefaultValue=false)]
        public EventThreshold EventEndpointCountThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventIngressFlowCountThreshold
        /// </summary>
        [DataMember(Name="eventIngressFlowCountThreshold", EmitDefaultValue=false)]
        public EventThreshold EventIngressFlowCountThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventIngressMsgRateThreshold
        /// </summary>
        [DataMember(Name="eventIngressMsgRateThreshold", EmitDefaultValue=false)]
        public EventThresholdByValue EventIngressMsgRateThreshold { get; set; }

        /// <summary>
        /// The threshold, in kilobytes, after which a message is considered to be large for the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1024&#x60;.
        /// </summary>
        /// <value>The threshold, in kilobytes, after which a message is considered to be large for the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1024&#x60;.</value>
        [DataMember(Name="eventLargeMsgThreshold", EmitDefaultValue=false)]
        public long? EventLargeMsgThreshold { get; set; }

        /// <summary>
        /// A prefix applied to all published Events in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>A prefix applied to all published Events in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="eventLogTag", EmitDefaultValue=false)]
        public string EventLogTag { get; set; }

        /// <summary>
        /// Gets or Sets EventMsgSpoolUsageThreshold
        /// </summary>
        [DataMember(Name="eventMsgSpoolUsageThreshold", EmitDefaultValue=false)]
        public EventThreshold EventMsgSpoolUsageThreshold { get; set; }

        /// <summary>
        /// Enable or disable Client level Event message publishing. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable Client level Event message publishing. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="eventPublishClientEnabled", EmitDefaultValue=false)]
        public bool? EventPublishClientEnabled { get; set; }

        /// <summary>
        /// Enable or disable Message VPN level Event message publishing. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable Message VPN level Event message publishing. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="eventPublishMsgVpnEnabled", EmitDefaultValue=false)]
        public bool? EventPublishMsgVpnEnabled { get; set; }


        /// <summary>
        /// Enable or disable Event publish topics in MQTT format. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable Event publish topics in MQTT format. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="eventPublishTopicFormatMqttEnabled", EmitDefaultValue=false)]
        public bool? EventPublishTopicFormatMqttEnabled { get; set; }

        /// <summary>
        /// Enable or disable Event publish topics in SMF format. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable Event publish topics in SMF format. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="eventPublishTopicFormatSmfEnabled", EmitDefaultValue=false)]
        public bool? EventPublishTopicFormatSmfEnabled { get; set; }

        /// <summary>
        /// Gets or Sets EventServiceAmqpConnectionCountThreshold
        /// </summary>
        [DataMember(Name="eventServiceAmqpConnectionCountThreshold", EmitDefaultValue=false)]
        public EventThreshold EventServiceAmqpConnectionCountThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventServiceMqttConnectionCountThreshold
        /// </summary>
        [DataMember(Name="eventServiceMqttConnectionCountThreshold", EmitDefaultValue=false)]
        public EventThreshold EventServiceMqttConnectionCountThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventServiceRestIncomingConnectionCountThreshold
        /// </summary>
        [DataMember(Name="eventServiceRestIncomingConnectionCountThreshold", EmitDefaultValue=false)]
        public EventThreshold EventServiceRestIncomingConnectionCountThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventServiceSmfConnectionCountThreshold
        /// </summary>
        [DataMember(Name="eventServiceSmfConnectionCountThreshold", EmitDefaultValue=false)]
        public EventThreshold EventServiceSmfConnectionCountThreshold { get; set; }

        /// <summary>
        /// Gets or Sets EventServiceWebConnectionCountThreshold
        /// </summary>
        [DataMember(Name="eventServiceWebConnectionCountThreshold", EmitDefaultValue=false)]
        public EventThreshold EventServiceWebConnectionCountThreshold { get; set; }

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
        /// Enable or disable the export of subscriptions in the Message VPN to other routers in the network over Neighbor links. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable the export of subscriptions in the Message VPN to other routers in the network over Neighbor links. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="exportSubscriptionsEnabled", EmitDefaultValue=false)]
        public bool? ExportSubscriptionsEnabled { get; set; }

        /// <summary>
        /// Enable or disable JNDI access for clients in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.2.
        /// </summary>
        /// <value>Enable or disable JNDI access for clients in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.2.</value>
        [DataMember(Name="jndiEnabled", EmitDefaultValue=false)]
        public bool? JndiEnabled { get; set; }

        /// <summary>
        /// The maximum number of client connections to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform.
        /// </summary>
        /// <value>The maximum number of client connections to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform.</value>
        [DataMember(Name="maxConnectionCount", EmitDefaultValue=false)]
        public long? MaxConnectionCount { get; set; }

        /// <summary>
        /// The maximum number of transmit flows that can be created in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;.
        /// </summary>
        /// <value>The maximum number of transmit flows that can be created in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;.</value>
        [DataMember(Name="maxEgressFlowCount", EmitDefaultValue=false)]
        public long? MaxEgressFlowCount { get; set; }

        /// <summary>
        /// The maximum number of Queues and Topic Endpoints that can be created in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;.
        /// </summary>
        /// <value>The maximum number of Queues and Topic Endpoints that can be created in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;.</value>
        [DataMember(Name="maxEndpointCount", EmitDefaultValue=false)]
        public long? MaxEndpointCount { get; set; }

        /// <summary>
        /// The maximum number of receive flows that can be created in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;.
        /// </summary>
        /// <value>The maximum number of receive flows that can be created in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;.</value>
        [DataMember(Name="maxIngressFlowCount", EmitDefaultValue=false)]
        public long? MaxIngressFlowCount { get; set; }

        /// <summary>
        /// The maximum message spool usage by the Message VPN, in megabytes. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;.
        /// </summary>
        /// <value>The maximum message spool usage by the Message VPN, in megabytes. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;.</value>
        [DataMember(Name="maxMsgSpoolUsage", EmitDefaultValue=false)]
        public long? MaxMsgSpoolUsage { get; set; }

        /// <summary>
        /// The maximum number of local client subscriptions that can be added to the Message VPN. This limit is not enforced when a subscription is added using a management interface, such as CLI or SEMP. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default varies by platform.
        /// </summary>
        /// <value>The maximum number of local client subscriptions that can be added to the Message VPN. This limit is not enforced when a subscription is added using a management interface, such as CLI or SEMP. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default varies by platform.</value>
        [DataMember(Name="maxSubscriptionCount", EmitDefaultValue=false)]
        public long? MaxSubscriptionCount { get; set; }

        /// <summary>
        /// The maximum number of transacted sessions that can be created in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default varies by platform.
        /// </summary>
        /// <value>The maximum number of transacted sessions that can be created in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default varies by platform.</value>
        [DataMember(Name="maxTransactedSessionCount", EmitDefaultValue=false)]
        public long? MaxTransactedSessionCount { get; set; }

        /// <summary>
        /// The maximum number of transactions that can be created in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default varies by platform.
        /// </summary>
        /// <value>The maximum number of transactions that can be created in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default varies by platform.</value>
        [DataMember(Name="maxTransactionCount", EmitDefaultValue=false)]
        public long? MaxTransactionCount { get; set; }

        /// <summary>
        /// The maximum total memory usage of the MQTT Retain feature for this Message VPN, in MB. If the maximum memory is reached, any arriving retain messages that require more memory are discarded. A value of -1 indicates that the memory is bounded only by the global max memory limit. A value of 0 prevents MQTT Retain from becoming operational. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;-1&#x60;. Available since 2.11.
        /// </summary>
        /// <value>The maximum total memory usage of the MQTT Retain feature for this Message VPN, in MB. If the maximum memory is reached, any arriving retain messages that require more memory are discarded. A value of -1 indicates that the memory is bounded only by the global max memory limit. A value of 0 prevents MQTT Retain from becoming operational. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;-1&#x60;. Available since 2.11.</value>
        [DataMember(Name="mqttRetainMaxMemory", EmitDefaultValue=false)]
        public int? MqttRetainMaxMemory { get; set; }

        /// <summary>
        /// The name of the Message VPN.
        /// </summary>
        /// <value>The name of the Message VPN.</value>
        [DataMember(Name="msgVpnName", EmitDefaultValue=false)]
        public string MsgVpnName { get; set; }

        /// <summary>
        /// The acknowledgement (ACK) propagation interval for the replication Bridge, in number of replicated messages. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;20&#x60;.
        /// </summary>
        /// <value>The acknowledgement (ACK) propagation interval for the replication Bridge, in number of replicated messages. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;20&#x60;.</value>
        [DataMember(Name="replicationAckPropagationIntervalMsgCount", EmitDefaultValue=false)]
        public long? ReplicationAckPropagationIntervalMsgCount { get; set; }

        /// <summary>
        /// The Client Username the replication Bridge uses to login to the remote Message VPN. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The Client Username the replication Bridge uses to login to the remote Message VPN. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="replicationBridgeAuthenticationBasicClientUsername", EmitDefaultValue=false)]
        public string ReplicationBridgeAuthenticationBasicClientUsername { get; set; }

        /// <summary>
        /// The password for the Client Username. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The password for the Client Username. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="replicationBridgeAuthenticationBasicPassword", EmitDefaultValue=false)]
        public string ReplicationBridgeAuthenticationBasicPassword { get; set; }

        /// <summary>
        /// The PEM formatted content for the client certificate used by this bridge to login to the Remote Message VPN. It must consist of a private key and between one and three certificates comprising the certificate trust chain. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.9.
        /// </summary>
        /// <value>The PEM formatted content for the client certificate used by this bridge to login to the Remote Message VPN. It must consist of a private key and between one and three certificates comprising the certificate trust chain. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.9.</value>
        [DataMember(Name="replicationBridgeAuthenticationClientCertContent", EmitDefaultValue=false)]
        public string ReplicationBridgeAuthenticationClientCertContent { get; set; }

        /// <summary>
        /// The password for the client certificate. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.9.
        /// </summary>
        /// <value>The password for the client certificate. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.9.</value>
        [DataMember(Name="replicationBridgeAuthenticationClientCertPassword", EmitDefaultValue=false)]
        public string ReplicationBridgeAuthenticationClientCertPassword { get; set; }


        /// <summary>
        /// Enable or disable use of compression for the replication Bridge. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable use of compression for the replication Bridge. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="replicationBridgeCompressedDataEnabled", EmitDefaultValue=false)]
        public bool? ReplicationBridgeCompressedDataEnabled { get; set; }

        /// <summary>
        /// The size of the window used for guaranteed messages published to the replication Bridge, in messages. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;255&#x60;.
        /// </summary>
        /// <value>The size of the window used for guaranteed messages published to the replication Bridge, in messages. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;255&#x60;.</value>
        [DataMember(Name="replicationBridgeEgressFlowWindowSize", EmitDefaultValue=false)]
        public long? ReplicationBridgeEgressFlowWindowSize { get; set; }

        /// <summary>
        /// The number of seconds that must pass before retrying the replication Bridge connection. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;3&#x60;.
        /// </summary>
        /// <value>The number of seconds that must pass before retrying the replication Bridge connection. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;3&#x60;.</value>
        [DataMember(Name="replicationBridgeRetryDelay", EmitDefaultValue=false)]
        public long? ReplicationBridgeRetryDelay { get; set; }

        /// <summary>
        /// Enable or disable use of encryption (TLS) for the replication Bridge connection. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable use of encryption (TLS) for the replication Bridge connection. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="replicationBridgeTlsEnabled", EmitDefaultValue=false)]
        public bool? ReplicationBridgeTlsEnabled { get; set; }

        /// <summary>
        /// The Client Profile for the unidirectional replication Bridge in the Message VPN. It is used only for the TCP parameters. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;#client-profile\&quot;&#x60;.
        /// </summary>
        /// <value>The Client Profile for the unidirectional replication Bridge in the Message VPN. It is used only for the TCP parameters. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;#client-profile\&quot;&#x60;.</value>
        [DataMember(Name="replicationBridgeUnidirectionalClientProfileName", EmitDefaultValue=false)]
        public string ReplicationBridgeUnidirectionalClientProfileName { get; set; }

        /// <summary>
        /// Enable or disable replication for the Message VPN. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable replication for the Message VPN. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="replicationEnabled", EmitDefaultValue=false)]
        public bool? ReplicationEnabled { get; set; }


        /// <summary>
        /// The maximum message spool usage by the replication Bridge local Queue (quota), in megabytes. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;60000&#x60;.
        /// </summary>
        /// <value>The maximum message spool usage by the replication Bridge local Queue (quota), in megabytes. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;60000&#x60;.</value>
        [DataMember(Name="replicationQueueMaxMsgSpoolUsage", EmitDefaultValue=false)]
        public long? ReplicationQueueMaxMsgSpoolUsage { get; set; }

        /// <summary>
        /// Enable or disable whether messages discarded on the replication Bridge local Queue are rejected back to the sender. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable whether messages discarded on the replication Bridge local Queue are rejected back to the sender. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="replicationQueueRejectMsgToSenderOnDiscardEnabled", EmitDefaultValue=false)]
        public bool? ReplicationQueueRejectMsgToSenderOnDiscardEnabled { get; set; }

        /// <summary>
        /// Enable or disable whether guaranteed messages published to synchronously replicated Topics are rejected back to the sender when synchronous replication becomes ineligible. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable whether guaranteed messages published to synchronously replicated Topics are rejected back to the sender when synchronous replication becomes ineligible. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="replicationRejectMsgWhenSyncIneligibleEnabled", EmitDefaultValue=false)]
        public bool? ReplicationRejectMsgWhenSyncIneligibleEnabled { get; set; }



        /// <summary>
        /// Enable or disable validation of the Common Name (CN) in the server certificate from the remote REST Consumer. If enabled, the Common Name is checked against the list of Trusted Common Names configured for the REST Consumer. Common Name validation is not performed if Server Certificate Name Validation is enabled, even if Common Name validation is enabled. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.17. Common Name validation has been replaced by Server Certificate Name validation.
        /// </summary>
        /// <value>Enable or disable validation of the Common Name (CN) in the server certificate from the remote REST Consumer. If enabled, the Common Name is checked against the list of Trusted Common Names configured for the REST Consumer. Common Name validation is not performed if Server Certificate Name Validation is enabled, even if Common Name validation is enabled. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Deprecated since 2.17. Common Name validation has been replaced by Server Certificate Name validation.</value>
        [DataMember(Name="restTlsServerCertEnforceTrustedCommonNameEnabled", EmitDefaultValue=false)]
        public bool? RestTlsServerCertEnforceTrustedCommonNameEnabled { get; set; }

        /// <summary>
        /// The maximum depth for a REST Consumer server certificate chain. The depth of a chain is defined as the number of signing CA certificates that are present in the chain back to a trusted self-signed root CA certificate. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;.
        /// </summary>
        /// <value>The maximum depth for a REST Consumer server certificate chain. The depth of a chain is defined as the number of signing CA certificates that are present in the chain back to a trusted self-signed root CA certificate. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;.</value>
        [DataMember(Name="restTlsServerCertMaxChainDepth", EmitDefaultValue=false)]
        public long? RestTlsServerCertMaxChainDepth { get; set; }

        /// <summary>
        /// Enable or disable validation of the \&quot;Not Before\&quot; and \&quot;Not After\&quot; validity dates in the REST Consumer server certificate. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable validation of the \&quot;Not Before\&quot; and \&quot;Not After\&quot; validity dates in the REST Consumer server certificate. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="restTlsServerCertValidateDateEnabled", EmitDefaultValue=false)]
        public bool? RestTlsServerCertValidateDateEnabled { get; set; }

        /// <summary>
        /// Enable or disable the standard TLS authentication mechanism of verifying the name used to connect to the remote REST Consumer. If enabled, the name used to connect to the remote REST Consumer is checked against the names specified in the certificate returned by the remote router. Legacy Common Name validation is not performed if Server Certificate Name Validation is enabled, even if Common Name validation is also enabled. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;. Available since 2.17.
        /// </summary>
        /// <value>Enable or disable the standard TLS authentication mechanism of verifying the name used to connect to the remote REST Consumer. If enabled, the name used to connect to the remote REST Consumer is checked against the names specified in the certificate returned by the remote router. Legacy Common Name validation is not performed if Server Certificate Name Validation is enabled, even if Common Name validation is also enabled. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;. Available since 2.17.</value>
        [DataMember(Name="restTlsServerCertValidateNameEnabled", EmitDefaultValue=false)]
        public bool? RestTlsServerCertValidateNameEnabled { get; set; }

        /// <summary>
        /// Enable or disable \&quot;admin client\&quot; SEMP over the message bus commands for the current Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable \&quot;admin client\&quot; SEMP over the message bus commands for the current Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="sempOverMsgBusAdminClientEnabled", EmitDefaultValue=false)]
        public bool? SempOverMsgBusAdminClientEnabled { get; set; }

        /// <summary>
        /// Enable or disable \&quot;admin distributed-cache\&quot; SEMP over the message bus commands for the current Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable \&quot;admin distributed-cache\&quot; SEMP over the message bus commands for the current Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="sempOverMsgBusAdminDistributedCacheEnabled", EmitDefaultValue=false)]
        public bool? SempOverMsgBusAdminDistributedCacheEnabled { get; set; }

        /// <summary>
        /// Enable or disable \&quot;admin\&quot; SEMP over the message bus commands for the current Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable \&quot;admin\&quot; SEMP over the message bus commands for the current Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="sempOverMsgBusAdminEnabled", EmitDefaultValue=false)]
        public bool? SempOverMsgBusAdminEnabled { get; set; }

        /// <summary>
        /// Enable or disable SEMP over the message bus for the current Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable SEMP over the message bus for the current Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="sempOverMsgBusEnabled", EmitDefaultValue=false)]
        public bool? SempOverMsgBusEnabled { get; set; }

        /// <summary>
        /// Enable or disable \&quot;show\&quot; SEMP over the message bus commands for the current Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable \&quot;show\&quot; SEMP over the message bus commands for the current Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="sempOverMsgBusShowEnabled", EmitDefaultValue=false)]
        public bool? SempOverMsgBusShowEnabled { get; set; }

        /// <summary>
        /// The maximum number of AMQP client connections that can be simultaneously connected to the Message VPN. This value may be higher than supported by the platform. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform. Available since 2.7.
        /// </summary>
        /// <value>The maximum number of AMQP client connections that can be simultaneously connected to the Message VPN. This value may be higher than supported by the platform. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform. Available since 2.7.</value>
        [DataMember(Name="serviceAmqpMaxConnectionCount", EmitDefaultValue=false)]
        public long? ServiceAmqpMaxConnectionCount { get; set; }

        /// <summary>
        /// Enable or disable the plain-text AMQP service in the Message VPN. Disabling causes clients connected to the corresponding listen-port to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.7.
        /// </summary>
        /// <value>Enable or disable the plain-text AMQP service in the Message VPN. Disabling causes clients connected to the corresponding listen-port to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.7.</value>
        [DataMember(Name="serviceAmqpPlainTextEnabled", EmitDefaultValue=false)]
        public bool? ServiceAmqpPlainTextEnabled { get; set; }

        /// <summary>
        /// The port number for plain-text AMQP clients that connect to the Message VPN. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceAmqpPlainTextEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.7.
        /// </summary>
        /// <value>The port number for plain-text AMQP clients that connect to the Message VPN. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceAmqpPlainTextEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.7.</value>
        [DataMember(Name="serviceAmqpPlainTextListenPort", EmitDefaultValue=false)]
        public long? ServiceAmqpPlainTextListenPort { get; set; }

        /// <summary>
        /// Enable or disable the use of encryption (TLS) for the AMQP service in the Message VPN. Disabling causes clients currently connected over TLS to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.7.
        /// </summary>
        /// <value>Enable or disable the use of encryption (TLS) for the AMQP service in the Message VPN. Disabling causes clients currently connected over TLS to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.7.</value>
        [DataMember(Name="serviceAmqpTlsEnabled", EmitDefaultValue=false)]
        public bool? ServiceAmqpTlsEnabled { get; set; }

        /// <summary>
        /// The port number for AMQP clients that connect to the Message VPN over TLS. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceAmqpTlsEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.7.
        /// </summary>
        /// <value>The port number for AMQP clients that connect to the Message VPN over TLS. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceAmqpTlsEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.7.</value>
        [DataMember(Name="serviceAmqpTlsListenPort", EmitDefaultValue=false)]
        public long? ServiceAmqpTlsListenPort { get; set; }


        /// <summary>
        /// The maximum number of MQTT client connections that can be simultaneously connected to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform. Available since 2.1.
        /// </summary>
        /// <value>The maximum number of MQTT client connections that can be simultaneously connected to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform. Available since 2.1.</value>
        [DataMember(Name="serviceMqttMaxConnectionCount", EmitDefaultValue=false)]
        public long? ServiceMqttMaxConnectionCount { get; set; }

        /// <summary>
        /// Enable or disable the plain-text MQTT service in the Message VPN. Disabling causes clients currently connected to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.1.
        /// </summary>
        /// <value>Enable or disable the plain-text MQTT service in the Message VPN. Disabling causes clients currently connected to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.1.</value>
        [DataMember(Name="serviceMqttPlainTextEnabled", EmitDefaultValue=false)]
        public bool? ServiceMqttPlainTextEnabled { get; set; }

        /// <summary>
        /// The port number for plain-text MQTT clients that connect to the Message VPN. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceMqttPlainTextEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.1.
        /// </summary>
        /// <value>The port number for plain-text MQTT clients that connect to the Message VPN. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceMqttPlainTextEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.1.</value>
        [DataMember(Name="serviceMqttPlainTextListenPort", EmitDefaultValue=false)]
        public long? ServiceMqttPlainTextListenPort { get; set; }

        /// <summary>
        /// Enable or disable the use of encryption (TLS) for the MQTT service in the Message VPN. Disabling causes clients currently connected over TLS to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.1.
        /// </summary>
        /// <value>Enable or disable the use of encryption (TLS) for the MQTT service in the Message VPN. Disabling causes clients currently connected over TLS to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.1.</value>
        [DataMember(Name="serviceMqttTlsEnabled", EmitDefaultValue=false)]
        public bool? ServiceMqttTlsEnabled { get; set; }

        /// <summary>
        /// The port number for MQTT clients that connect to the Message VPN over TLS. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceMqttTlsEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.1.
        /// </summary>
        /// <value>The port number for MQTT clients that connect to the Message VPN over TLS. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceMqttTlsEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.1.</value>
        [DataMember(Name="serviceMqttTlsListenPort", EmitDefaultValue=false)]
        public long? ServiceMqttTlsListenPort { get; set; }

        /// <summary>
        /// Enable or disable the use of encrypted WebSocket (WebSocket over TLS) for the MQTT service in the Message VPN. Disabling causes clients currently connected by encrypted WebSocket to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.1.
        /// </summary>
        /// <value>Enable or disable the use of encrypted WebSocket (WebSocket over TLS) for the MQTT service in the Message VPN. Disabling causes clients currently connected by encrypted WebSocket to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.1.</value>
        [DataMember(Name="serviceMqttTlsWebSocketEnabled", EmitDefaultValue=false)]
        public bool? ServiceMqttTlsWebSocketEnabled { get; set; }

        /// <summary>
        /// The port number for MQTT clients that connect to the Message VPN using WebSocket over TLS. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceMqttTlsWebSocketEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.1.
        /// </summary>
        /// <value>The port number for MQTT clients that connect to the Message VPN using WebSocket over TLS. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceMqttTlsWebSocketEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.1.</value>
        [DataMember(Name="serviceMqttTlsWebSocketListenPort", EmitDefaultValue=false)]
        public long? ServiceMqttTlsWebSocketListenPort { get; set; }

        /// <summary>
        /// Enable or disable the use of WebSocket for the MQTT service in the Message VPN. Disabling causes clients currently connected by WebSocket to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.1.
        /// </summary>
        /// <value>Enable or disable the use of WebSocket for the MQTT service in the Message VPN. Disabling causes clients currently connected by WebSocket to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. Available since 2.1.</value>
        [DataMember(Name="serviceMqttWebSocketEnabled", EmitDefaultValue=false)]
        public bool? ServiceMqttWebSocketEnabled { get; set; }

        /// <summary>
        /// The port number for plain-text MQTT clients that connect to the Message VPN using WebSocket. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceMqttWebSocketEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.1.
        /// </summary>
        /// <value>The port number for plain-text MQTT clients that connect to the Message VPN using WebSocket. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceMqttWebSocketEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. Available since 2.1.</value>
        [DataMember(Name="serviceMqttWebSocketListenPort", EmitDefaultValue=false)]
        public long? ServiceMqttWebSocketListenPort { get; set; }



        /// <summary>
        /// The maximum number of REST incoming client connections that can be simultaneously connected to the Message VPN. This value may be higher than supported by the platform. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform.
        /// </summary>
        /// <value>The maximum number of REST incoming client connections that can be simultaneously connected to the Message VPN. This value may be higher than supported by the platform. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform.</value>
        [DataMember(Name="serviceRestIncomingMaxConnectionCount", EmitDefaultValue=false)]
        public long? ServiceRestIncomingMaxConnectionCount { get; set; }

        /// <summary>
        /// Enable or disable the plain-text REST service for incoming clients in the Message VPN. Disabling causes clients currently connected to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable the plain-text REST service for incoming clients in the Message VPN. Disabling causes clients currently connected to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="serviceRestIncomingPlainTextEnabled", EmitDefaultValue=false)]
        public bool? ServiceRestIncomingPlainTextEnabled { get; set; }

        /// <summary>
        /// The port number for incoming plain-text REST clients that connect to the Message VPN. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceRestIncomingPlainTextEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;.
        /// </summary>
        /// <value>The port number for incoming plain-text REST clients that connect to the Message VPN. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceRestIncomingPlainTextEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;.</value>
        [DataMember(Name="serviceRestIncomingPlainTextListenPort", EmitDefaultValue=false)]
        public long? ServiceRestIncomingPlainTextListenPort { get; set; }

        /// <summary>
        /// Enable or disable the use of encryption (TLS) for the REST service for incoming clients in the Message VPN. Disabling causes clients currently connected over TLS to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable the use of encryption (TLS) for the REST service for incoming clients in the Message VPN. Disabling causes clients currently connected over TLS to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="serviceRestIncomingTlsEnabled", EmitDefaultValue=false)]
        public bool? ServiceRestIncomingTlsEnabled { get; set; }

        /// <summary>
        /// The port number for incoming REST clients that connect to the Message VPN over TLS. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceRestIncomingTlsEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;.
        /// </summary>
        /// <value>The port number for incoming REST clients that connect to the Message VPN over TLS. The port must be unique across the message backbone. A value of 0 means that the listen-port is unassigned and cannot be enabled. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceRestIncomingTlsEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;.</value>
        [DataMember(Name="serviceRestIncomingTlsListenPort", EmitDefaultValue=false)]
        public long? ServiceRestIncomingTlsListenPort { get; set; }


        /// <summary>
        /// The maximum number of REST Consumer (outgoing) client connections that can be simultaneously connected to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default varies by platform.
        /// </summary>
        /// <value>The maximum number of REST Consumer (outgoing) client connections that can be simultaneously connected to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default varies by platform.</value>
        [DataMember(Name="serviceRestOutgoingMaxConnectionCount", EmitDefaultValue=false)]
        public long? ServiceRestOutgoingMaxConnectionCount { get; set; }

        /// <summary>
        /// The maximum number of SMF client connections that can be simultaneously connected to the Message VPN. This value may be higher than supported by the platform. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default varies by platform.
        /// </summary>
        /// <value>The maximum number of SMF client connections that can be simultaneously connected to the Message VPN. This value may be higher than supported by the platform. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default varies by platform.</value>
        [DataMember(Name="serviceSmfMaxConnectionCount", EmitDefaultValue=false)]
        public long? ServiceSmfMaxConnectionCount { get; set; }

        /// <summary>
        /// Enable or disable the plain-text SMF service in the Message VPN. Disabling causes clients currently connected to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable the plain-text SMF service in the Message VPN. Disabling causes clients currently connected to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="serviceSmfPlainTextEnabled", EmitDefaultValue=false)]
        public bool? ServiceSmfPlainTextEnabled { get; set; }

        /// <summary>
        /// Enable or disable the use of encryption (TLS) for the SMF service in the Message VPN. Disabling causes clients currently connected over TLS to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable the use of encryption (TLS) for the SMF service in the Message VPN. Disabling causes clients currently connected over TLS to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="serviceSmfTlsEnabled", EmitDefaultValue=false)]
        public bool? ServiceSmfTlsEnabled { get; set; }


        /// <summary>
        /// The maximum number of Web Transport client connections that can be simultaneously connected to the Message VPN. This value may be higher than supported by the platform. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform.
        /// </summary>
        /// <value>The maximum number of Web Transport client connections that can be simultaneously connected to the Message VPN. This value may be higher than supported by the platform. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform.</value>
        [DataMember(Name="serviceWebMaxConnectionCount", EmitDefaultValue=false)]
        public long? ServiceWebMaxConnectionCount { get; set; }

        /// <summary>
        /// Enable or disable the plain-text Web Transport service in the Message VPN. Disabling causes clients currently connected to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable the plain-text Web Transport service in the Message VPN. Disabling causes clients currently connected to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="serviceWebPlainTextEnabled", EmitDefaultValue=false)]
        public bool? ServiceWebPlainTextEnabled { get; set; }

        /// <summary>
        /// Enable or disable the use of TLS for the Web Transport service in the Message VPN. Disabling causes clients currently connected over TLS to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable the use of TLS for the Web Transport service in the Message VPN. Disabling causes clients currently connected over TLS to be disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="serviceWebTlsEnabled", EmitDefaultValue=false)]
        public bool? ServiceWebTlsEnabled { get; set; }

        /// <summary>
        /// Enable or disable the allowing of TLS SMF clients to downgrade their connections to plain-text connections. Changing this will not affect existing connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable the allowing of TLS SMF clients to downgrade their connections to plain-text connections. Changing this will not affect existing connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="tlsAllowDowngradeToPlainTextEnabled", EmitDefaultValue=false)]
        public bool? TlsAllowDowngradeToPlainTextEnabled { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class MsgVpn {\n");
            sb.Append("  Alias: ").Append(Alias).Append("\n");
            sb.Append("  AuthenticationBasicEnabled: ").Append(AuthenticationBasicEnabled).Append("\n");
            sb.Append("  AuthenticationBasicProfileName: ").Append(AuthenticationBasicProfileName).Append("\n");
            sb.Append("  AuthenticationBasicRadiusDomain: ").Append(AuthenticationBasicRadiusDomain).Append("\n");
            sb.Append("  AuthenticationBasicType: ").Append(AuthenticationBasicType).Append("\n");
            sb.Append("  AuthenticationClientCertAllowApiProvidedUsernameEnabled: ").Append(AuthenticationClientCertAllowApiProvidedUsernameEnabled).Append("\n");
            sb.Append("  AuthenticationClientCertCertificateMatchingRulesEnabled: ").Append(AuthenticationClientCertCertificateMatchingRulesEnabled).Append("\n");
            sb.Append("  AuthenticationClientCertEnabled: ").Append(AuthenticationClientCertEnabled).Append("\n");
            sb.Append("  AuthenticationClientCertMaxChainDepth: ").Append(AuthenticationClientCertMaxChainDepth).Append("\n");
            sb.Append("  AuthenticationClientCertRevocationCheckMode: ").Append(AuthenticationClientCertRevocationCheckMode).Append("\n");
            sb.Append("  AuthenticationClientCertUsernameSource: ").Append(AuthenticationClientCertUsernameSource).Append("\n");
            sb.Append("  AuthenticationClientCertValidateDateEnabled: ").Append(AuthenticationClientCertValidateDateEnabled).Append("\n");
            sb.Append("  AuthenticationKerberosAllowApiProvidedUsernameEnabled: ").Append(AuthenticationKerberosAllowApiProvidedUsernameEnabled).Append("\n");
            sb.Append("  AuthenticationKerberosEnabled: ").Append(AuthenticationKerberosEnabled).Append("\n");
            sb.Append("  AuthenticationOauthDefaultProfileName: ").Append(AuthenticationOauthDefaultProfileName).Append("\n");
            sb.Append("  AuthenticationOauthDefaultProviderName: ").Append(AuthenticationOauthDefaultProviderName).Append("\n");
            sb.Append("  AuthenticationOauthEnabled: ").Append(AuthenticationOauthEnabled).Append("\n");
            sb.Append("  AuthorizationLdapGroupMembershipAttributeName: ").Append(AuthorizationLdapGroupMembershipAttributeName).Append("\n");
            sb.Append("  AuthorizationLdapTrimClientUsernameDomainEnabled: ").Append(AuthorizationLdapTrimClientUsernameDomainEnabled).Append("\n");
            sb.Append("  AuthorizationProfileName: ").Append(AuthorizationProfileName).Append("\n");
            sb.Append("  AuthorizationType: ").Append(AuthorizationType).Append("\n");
            sb.Append("  BridgingTlsServerCertEnforceTrustedCommonNameEnabled: ").Append(BridgingTlsServerCertEnforceTrustedCommonNameEnabled).Append("\n");
            sb.Append("  BridgingTlsServerCertMaxChainDepth: ").Append(BridgingTlsServerCertMaxChainDepth).Append("\n");
            sb.Append("  BridgingTlsServerCertValidateDateEnabled: ").Append(BridgingTlsServerCertValidateDateEnabled).Append("\n");
            sb.Append("  BridgingTlsServerCertValidateNameEnabled: ").Append(BridgingTlsServerCertValidateNameEnabled).Append("\n");
            sb.Append("  DistributedCacheManagementEnabled: ").Append(DistributedCacheManagementEnabled).Append("\n");
            sb.Append("  DmrEnabled: ").Append(DmrEnabled).Append("\n");
            sb.Append("  Enabled: ").Append(Enabled).Append("\n");
            sb.Append("  EventConnectionCountThreshold: ").Append(EventConnectionCountThreshold).Append("\n");
            sb.Append("  EventEgressFlowCountThreshold: ").Append(EventEgressFlowCountThreshold).Append("\n");
            sb.Append("  EventEgressMsgRateThreshold: ").Append(EventEgressMsgRateThreshold).Append("\n");
            sb.Append("  EventEndpointCountThreshold: ").Append(EventEndpointCountThreshold).Append("\n");
            sb.Append("  EventIngressFlowCountThreshold: ").Append(EventIngressFlowCountThreshold).Append("\n");
            sb.Append("  EventIngressMsgRateThreshold: ").Append(EventIngressMsgRateThreshold).Append("\n");
            sb.Append("  EventLargeMsgThreshold: ").Append(EventLargeMsgThreshold).Append("\n");
            sb.Append("  EventLogTag: ").Append(EventLogTag).Append("\n");
            sb.Append("  EventMsgSpoolUsageThreshold: ").Append(EventMsgSpoolUsageThreshold).Append("\n");
            sb.Append("  EventPublishClientEnabled: ").Append(EventPublishClientEnabled).Append("\n");
            sb.Append("  EventPublishMsgVpnEnabled: ").Append(EventPublishMsgVpnEnabled).Append("\n");
            sb.Append("  EventPublishSubscriptionMode: ").Append(EventPublishSubscriptionMode).Append("\n");
            sb.Append("  EventPublishTopicFormatMqttEnabled: ").Append(EventPublishTopicFormatMqttEnabled).Append("\n");
            sb.Append("  EventPublishTopicFormatSmfEnabled: ").Append(EventPublishTopicFormatSmfEnabled).Append("\n");
            sb.Append("  EventServiceAmqpConnectionCountThreshold: ").Append(EventServiceAmqpConnectionCountThreshold).Append("\n");
            sb.Append("  EventServiceMqttConnectionCountThreshold: ").Append(EventServiceMqttConnectionCountThreshold).Append("\n");
            sb.Append("  EventServiceRestIncomingConnectionCountThreshold: ").Append(EventServiceRestIncomingConnectionCountThreshold).Append("\n");
            sb.Append("  EventServiceSmfConnectionCountThreshold: ").Append(EventServiceSmfConnectionCountThreshold).Append("\n");
            sb.Append("  EventServiceWebConnectionCountThreshold: ").Append(EventServiceWebConnectionCountThreshold).Append("\n");
            sb.Append("  EventSubscriptionCountThreshold: ").Append(EventSubscriptionCountThreshold).Append("\n");
            sb.Append("  EventTransactedSessionCountThreshold: ").Append(EventTransactedSessionCountThreshold).Append("\n");
            sb.Append("  EventTransactionCountThreshold: ").Append(EventTransactionCountThreshold).Append("\n");
            sb.Append("  ExportSubscriptionsEnabled: ").Append(ExportSubscriptionsEnabled).Append("\n");
            sb.Append("  JndiEnabled: ").Append(JndiEnabled).Append("\n");
            sb.Append("  MaxConnectionCount: ").Append(MaxConnectionCount).Append("\n");
            sb.Append("  MaxEgressFlowCount: ").Append(MaxEgressFlowCount).Append("\n");
            sb.Append("  MaxEndpointCount: ").Append(MaxEndpointCount).Append("\n");
            sb.Append("  MaxIngressFlowCount: ").Append(MaxIngressFlowCount).Append("\n");
            sb.Append("  MaxMsgSpoolUsage: ").Append(MaxMsgSpoolUsage).Append("\n");
            sb.Append("  MaxSubscriptionCount: ").Append(MaxSubscriptionCount).Append("\n");
            sb.Append("  MaxTransactedSessionCount: ").Append(MaxTransactedSessionCount).Append("\n");
            sb.Append("  MaxTransactionCount: ").Append(MaxTransactionCount).Append("\n");
            sb.Append("  MqttRetainMaxMemory: ").Append(MqttRetainMaxMemory).Append("\n");
            sb.Append("  MsgVpnName: ").Append(MsgVpnName).Append("\n");
            sb.Append("  ReplicationAckPropagationIntervalMsgCount: ").Append(ReplicationAckPropagationIntervalMsgCount).Append("\n");
            sb.Append("  ReplicationBridgeAuthenticationBasicClientUsername: ").Append(ReplicationBridgeAuthenticationBasicClientUsername).Append("\n");
            sb.Append("  ReplicationBridgeAuthenticationBasicPassword: ").Append(ReplicationBridgeAuthenticationBasicPassword).Append("\n");
            sb.Append("  ReplicationBridgeAuthenticationClientCertContent: ").Append(ReplicationBridgeAuthenticationClientCertContent).Append("\n");
            sb.Append("  ReplicationBridgeAuthenticationClientCertPassword: ").Append(ReplicationBridgeAuthenticationClientCertPassword).Append("\n");
            sb.Append("  ReplicationBridgeAuthenticationScheme: ").Append(ReplicationBridgeAuthenticationScheme).Append("\n");
            sb.Append("  ReplicationBridgeCompressedDataEnabled: ").Append(ReplicationBridgeCompressedDataEnabled).Append("\n");
            sb.Append("  ReplicationBridgeEgressFlowWindowSize: ").Append(ReplicationBridgeEgressFlowWindowSize).Append("\n");
            sb.Append("  ReplicationBridgeRetryDelay: ").Append(ReplicationBridgeRetryDelay).Append("\n");
            sb.Append("  ReplicationBridgeTlsEnabled: ").Append(ReplicationBridgeTlsEnabled).Append("\n");
            sb.Append("  ReplicationBridgeUnidirectionalClientProfileName: ").Append(ReplicationBridgeUnidirectionalClientProfileName).Append("\n");
            sb.Append("  ReplicationEnabled: ").Append(ReplicationEnabled).Append("\n");
            sb.Append("  ReplicationEnabledQueueBehavior: ").Append(ReplicationEnabledQueueBehavior).Append("\n");
            sb.Append("  ReplicationQueueMaxMsgSpoolUsage: ").Append(ReplicationQueueMaxMsgSpoolUsage).Append("\n");
            sb.Append("  ReplicationQueueRejectMsgToSenderOnDiscardEnabled: ").Append(ReplicationQueueRejectMsgToSenderOnDiscardEnabled).Append("\n");
            sb.Append("  ReplicationRejectMsgWhenSyncIneligibleEnabled: ").Append(ReplicationRejectMsgWhenSyncIneligibleEnabled).Append("\n");
            sb.Append("  ReplicationRole: ").Append(ReplicationRole).Append("\n");
            sb.Append("  ReplicationTransactionMode: ").Append(ReplicationTransactionMode).Append("\n");
            sb.Append("  RestTlsServerCertEnforceTrustedCommonNameEnabled: ").Append(RestTlsServerCertEnforceTrustedCommonNameEnabled).Append("\n");
            sb.Append("  RestTlsServerCertMaxChainDepth: ").Append(RestTlsServerCertMaxChainDepth).Append("\n");
            sb.Append("  RestTlsServerCertValidateDateEnabled: ").Append(RestTlsServerCertValidateDateEnabled).Append("\n");
            sb.Append("  RestTlsServerCertValidateNameEnabled: ").Append(RestTlsServerCertValidateNameEnabled).Append("\n");
            sb.Append("  SempOverMsgBusAdminClientEnabled: ").Append(SempOverMsgBusAdminClientEnabled).Append("\n");
            sb.Append("  SempOverMsgBusAdminDistributedCacheEnabled: ").Append(SempOverMsgBusAdminDistributedCacheEnabled).Append("\n");
            sb.Append("  SempOverMsgBusAdminEnabled: ").Append(SempOverMsgBusAdminEnabled).Append("\n");
            sb.Append("  SempOverMsgBusEnabled: ").Append(SempOverMsgBusEnabled).Append("\n");
            sb.Append("  SempOverMsgBusShowEnabled: ").Append(SempOverMsgBusShowEnabled).Append("\n");
            sb.Append("  ServiceAmqpMaxConnectionCount: ").Append(ServiceAmqpMaxConnectionCount).Append("\n");
            sb.Append("  ServiceAmqpPlainTextEnabled: ").Append(ServiceAmqpPlainTextEnabled).Append("\n");
            sb.Append("  ServiceAmqpPlainTextListenPort: ").Append(ServiceAmqpPlainTextListenPort).Append("\n");
            sb.Append("  ServiceAmqpTlsEnabled: ").Append(ServiceAmqpTlsEnabled).Append("\n");
            sb.Append("  ServiceAmqpTlsListenPort: ").Append(ServiceAmqpTlsListenPort).Append("\n");
            sb.Append("  ServiceMqttAuthenticationClientCertRequest: ").Append(ServiceMqttAuthenticationClientCertRequest).Append("\n");
            sb.Append("  ServiceMqttMaxConnectionCount: ").Append(ServiceMqttMaxConnectionCount).Append("\n");
            sb.Append("  ServiceMqttPlainTextEnabled: ").Append(ServiceMqttPlainTextEnabled).Append("\n");
            sb.Append("  ServiceMqttPlainTextListenPort: ").Append(ServiceMqttPlainTextListenPort).Append("\n");
            sb.Append("  ServiceMqttTlsEnabled: ").Append(ServiceMqttTlsEnabled).Append("\n");
            sb.Append("  ServiceMqttTlsListenPort: ").Append(ServiceMqttTlsListenPort).Append("\n");
            sb.Append("  ServiceMqttTlsWebSocketEnabled: ").Append(ServiceMqttTlsWebSocketEnabled).Append("\n");
            sb.Append("  ServiceMqttTlsWebSocketListenPort: ").Append(ServiceMqttTlsWebSocketListenPort).Append("\n");
            sb.Append("  ServiceMqttWebSocketEnabled: ").Append(ServiceMqttWebSocketEnabled).Append("\n");
            sb.Append("  ServiceMqttWebSocketListenPort: ").Append(ServiceMqttWebSocketListenPort).Append("\n");
            sb.Append("  ServiceRestIncomingAuthenticationClientCertRequest: ").Append(ServiceRestIncomingAuthenticationClientCertRequest).Append("\n");
            sb.Append("  ServiceRestIncomingAuthorizationHeaderHandling: ").Append(ServiceRestIncomingAuthorizationHeaderHandling).Append("\n");
            sb.Append("  ServiceRestIncomingMaxConnectionCount: ").Append(ServiceRestIncomingMaxConnectionCount).Append("\n");
            sb.Append("  ServiceRestIncomingPlainTextEnabled: ").Append(ServiceRestIncomingPlainTextEnabled).Append("\n");
            sb.Append("  ServiceRestIncomingPlainTextListenPort: ").Append(ServiceRestIncomingPlainTextListenPort).Append("\n");
            sb.Append("  ServiceRestIncomingTlsEnabled: ").Append(ServiceRestIncomingTlsEnabled).Append("\n");
            sb.Append("  ServiceRestIncomingTlsListenPort: ").Append(ServiceRestIncomingTlsListenPort).Append("\n");
            sb.Append("  ServiceRestMode: ").Append(ServiceRestMode).Append("\n");
            sb.Append("  ServiceRestOutgoingMaxConnectionCount: ").Append(ServiceRestOutgoingMaxConnectionCount).Append("\n");
            sb.Append("  ServiceSmfMaxConnectionCount: ").Append(ServiceSmfMaxConnectionCount).Append("\n");
            sb.Append("  ServiceSmfPlainTextEnabled: ").Append(ServiceSmfPlainTextEnabled).Append("\n");
            sb.Append("  ServiceSmfTlsEnabled: ").Append(ServiceSmfTlsEnabled).Append("\n");
            sb.Append("  ServiceWebAuthenticationClientCertRequest: ").Append(ServiceWebAuthenticationClientCertRequest).Append("\n");
            sb.Append("  ServiceWebMaxConnectionCount: ").Append(ServiceWebMaxConnectionCount).Append("\n");
            sb.Append("  ServiceWebPlainTextEnabled: ").Append(ServiceWebPlainTextEnabled).Append("\n");
            sb.Append("  ServiceWebTlsEnabled: ").Append(ServiceWebTlsEnabled).Append("\n");
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
            return this.Equals(input as MsgVpn);
        }

        /// <summary>
        /// Returns true if MsgVpn instances are equal
        /// </summary>
        /// <param name="input">Instance of MsgVpn to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(MsgVpn input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Alias == input.Alias ||
                    (this.Alias != null &&
                    this.Alias.Equals(input.Alias))
                ) && 
                (
                    this.AuthenticationBasicEnabled == input.AuthenticationBasicEnabled ||
                    (this.AuthenticationBasicEnabled != null &&
                    this.AuthenticationBasicEnabled.Equals(input.AuthenticationBasicEnabled))
                ) && 
                (
                    this.AuthenticationBasicProfileName == input.AuthenticationBasicProfileName ||
                    (this.AuthenticationBasicProfileName != null &&
                    this.AuthenticationBasicProfileName.Equals(input.AuthenticationBasicProfileName))
                ) && 
                (
                    this.AuthenticationBasicRadiusDomain == input.AuthenticationBasicRadiusDomain ||
                    (this.AuthenticationBasicRadiusDomain != null &&
                    this.AuthenticationBasicRadiusDomain.Equals(input.AuthenticationBasicRadiusDomain))
                ) && 
                (
                    this.AuthenticationBasicType == input.AuthenticationBasicType ||
                    (this.AuthenticationBasicType != null &&
                    this.AuthenticationBasicType.Equals(input.AuthenticationBasicType))
                ) && 
                (
                    this.AuthenticationClientCertAllowApiProvidedUsernameEnabled == input.AuthenticationClientCertAllowApiProvidedUsernameEnabled ||
                    (this.AuthenticationClientCertAllowApiProvidedUsernameEnabled != null &&
                    this.AuthenticationClientCertAllowApiProvidedUsernameEnabled.Equals(input.AuthenticationClientCertAllowApiProvidedUsernameEnabled))
                ) && 
                (
                    this.AuthenticationClientCertCertificateMatchingRulesEnabled == input.AuthenticationClientCertCertificateMatchingRulesEnabled ||
                    (this.AuthenticationClientCertCertificateMatchingRulesEnabled != null &&
                    this.AuthenticationClientCertCertificateMatchingRulesEnabled.Equals(input.AuthenticationClientCertCertificateMatchingRulesEnabled))
                ) && 
                (
                    this.AuthenticationClientCertEnabled == input.AuthenticationClientCertEnabled ||
                    (this.AuthenticationClientCertEnabled != null &&
                    this.AuthenticationClientCertEnabled.Equals(input.AuthenticationClientCertEnabled))
                ) && 
                (
                    this.AuthenticationClientCertMaxChainDepth == input.AuthenticationClientCertMaxChainDepth ||
                    (this.AuthenticationClientCertMaxChainDepth != null &&
                    this.AuthenticationClientCertMaxChainDepth.Equals(input.AuthenticationClientCertMaxChainDepth))
                ) && 
                (
                    this.AuthenticationClientCertRevocationCheckMode == input.AuthenticationClientCertRevocationCheckMode ||
                    (this.AuthenticationClientCertRevocationCheckMode != null &&
                    this.AuthenticationClientCertRevocationCheckMode.Equals(input.AuthenticationClientCertRevocationCheckMode))
                ) && 
                (
                    this.AuthenticationClientCertUsernameSource == input.AuthenticationClientCertUsernameSource ||
                    (this.AuthenticationClientCertUsernameSource != null &&
                    this.AuthenticationClientCertUsernameSource.Equals(input.AuthenticationClientCertUsernameSource))
                ) && 
                (
                    this.AuthenticationClientCertValidateDateEnabled == input.AuthenticationClientCertValidateDateEnabled ||
                    (this.AuthenticationClientCertValidateDateEnabled != null &&
                    this.AuthenticationClientCertValidateDateEnabled.Equals(input.AuthenticationClientCertValidateDateEnabled))
                ) && 
                (
                    this.AuthenticationKerberosAllowApiProvidedUsernameEnabled == input.AuthenticationKerberosAllowApiProvidedUsernameEnabled ||
                    (this.AuthenticationKerberosAllowApiProvidedUsernameEnabled != null &&
                    this.AuthenticationKerberosAllowApiProvidedUsernameEnabled.Equals(input.AuthenticationKerberosAllowApiProvidedUsernameEnabled))
                ) && 
                (
                    this.AuthenticationKerberosEnabled == input.AuthenticationKerberosEnabled ||
                    (this.AuthenticationKerberosEnabled != null &&
                    this.AuthenticationKerberosEnabled.Equals(input.AuthenticationKerberosEnabled))
                ) && 
                (
                    this.AuthenticationOauthDefaultProfileName == input.AuthenticationOauthDefaultProfileName ||
                    (this.AuthenticationOauthDefaultProfileName != null &&
                    this.AuthenticationOauthDefaultProfileName.Equals(input.AuthenticationOauthDefaultProfileName))
                ) && 
                (
                    this.AuthenticationOauthDefaultProviderName == input.AuthenticationOauthDefaultProviderName ||
                    (this.AuthenticationOauthDefaultProviderName != null &&
                    this.AuthenticationOauthDefaultProviderName.Equals(input.AuthenticationOauthDefaultProviderName))
                ) && 
                (
                    this.AuthenticationOauthEnabled == input.AuthenticationOauthEnabled ||
                    (this.AuthenticationOauthEnabled != null &&
                    this.AuthenticationOauthEnabled.Equals(input.AuthenticationOauthEnabled))
                ) && 
                (
                    this.AuthorizationLdapGroupMembershipAttributeName == input.AuthorizationLdapGroupMembershipAttributeName ||
                    (this.AuthorizationLdapGroupMembershipAttributeName != null &&
                    this.AuthorizationLdapGroupMembershipAttributeName.Equals(input.AuthorizationLdapGroupMembershipAttributeName))
                ) && 
                (
                    this.AuthorizationLdapTrimClientUsernameDomainEnabled == input.AuthorizationLdapTrimClientUsernameDomainEnabled ||
                    (this.AuthorizationLdapTrimClientUsernameDomainEnabled != null &&
                    this.AuthorizationLdapTrimClientUsernameDomainEnabled.Equals(input.AuthorizationLdapTrimClientUsernameDomainEnabled))
                ) && 
                (
                    this.AuthorizationProfileName == input.AuthorizationProfileName ||
                    (this.AuthorizationProfileName != null &&
                    this.AuthorizationProfileName.Equals(input.AuthorizationProfileName))
                ) && 
                (
                    this.AuthorizationType == input.AuthorizationType ||
                    (this.AuthorizationType != null &&
                    this.AuthorizationType.Equals(input.AuthorizationType))
                ) && 
                (
                    this.BridgingTlsServerCertEnforceTrustedCommonNameEnabled == input.BridgingTlsServerCertEnforceTrustedCommonNameEnabled ||
                    (this.BridgingTlsServerCertEnforceTrustedCommonNameEnabled != null &&
                    this.BridgingTlsServerCertEnforceTrustedCommonNameEnabled.Equals(input.BridgingTlsServerCertEnforceTrustedCommonNameEnabled))
                ) && 
                (
                    this.BridgingTlsServerCertMaxChainDepth == input.BridgingTlsServerCertMaxChainDepth ||
                    (this.BridgingTlsServerCertMaxChainDepth != null &&
                    this.BridgingTlsServerCertMaxChainDepth.Equals(input.BridgingTlsServerCertMaxChainDepth))
                ) && 
                (
                    this.BridgingTlsServerCertValidateDateEnabled == input.BridgingTlsServerCertValidateDateEnabled ||
                    (this.BridgingTlsServerCertValidateDateEnabled != null &&
                    this.BridgingTlsServerCertValidateDateEnabled.Equals(input.BridgingTlsServerCertValidateDateEnabled))
                ) && 
                (
                    this.BridgingTlsServerCertValidateNameEnabled == input.BridgingTlsServerCertValidateNameEnabled ||
                    (this.BridgingTlsServerCertValidateNameEnabled != null &&
                    this.BridgingTlsServerCertValidateNameEnabled.Equals(input.BridgingTlsServerCertValidateNameEnabled))
                ) && 
                (
                    this.DistributedCacheManagementEnabled == input.DistributedCacheManagementEnabled ||
                    (this.DistributedCacheManagementEnabled != null &&
                    this.DistributedCacheManagementEnabled.Equals(input.DistributedCacheManagementEnabled))
                ) && 
                (
                    this.DmrEnabled == input.DmrEnabled ||
                    (this.DmrEnabled != null &&
                    this.DmrEnabled.Equals(input.DmrEnabled))
                ) && 
                (
                    this.Enabled == input.Enabled ||
                    (this.Enabled != null &&
                    this.Enabled.Equals(input.Enabled))
                ) && 
                (
                    this.EventConnectionCountThreshold == input.EventConnectionCountThreshold ||
                    (this.EventConnectionCountThreshold != null &&
                    this.EventConnectionCountThreshold.Equals(input.EventConnectionCountThreshold))
                ) && 
                (
                    this.EventEgressFlowCountThreshold == input.EventEgressFlowCountThreshold ||
                    (this.EventEgressFlowCountThreshold != null &&
                    this.EventEgressFlowCountThreshold.Equals(input.EventEgressFlowCountThreshold))
                ) && 
                (
                    this.EventEgressMsgRateThreshold == input.EventEgressMsgRateThreshold ||
                    (this.EventEgressMsgRateThreshold != null &&
                    this.EventEgressMsgRateThreshold.Equals(input.EventEgressMsgRateThreshold))
                ) && 
                (
                    this.EventEndpointCountThreshold == input.EventEndpointCountThreshold ||
                    (this.EventEndpointCountThreshold != null &&
                    this.EventEndpointCountThreshold.Equals(input.EventEndpointCountThreshold))
                ) && 
                (
                    this.EventIngressFlowCountThreshold == input.EventIngressFlowCountThreshold ||
                    (this.EventIngressFlowCountThreshold != null &&
                    this.EventIngressFlowCountThreshold.Equals(input.EventIngressFlowCountThreshold))
                ) && 
                (
                    this.EventIngressMsgRateThreshold == input.EventIngressMsgRateThreshold ||
                    (this.EventIngressMsgRateThreshold != null &&
                    this.EventIngressMsgRateThreshold.Equals(input.EventIngressMsgRateThreshold))
                ) && 
                (
                    this.EventLargeMsgThreshold == input.EventLargeMsgThreshold ||
                    (this.EventLargeMsgThreshold != null &&
                    this.EventLargeMsgThreshold.Equals(input.EventLargeMsgThreshold))
                ) && 
                (
                    this.EventLogTag == input.EventLogTag ||
                    (this.EventLogTag != null &&
                    this.EventLogTag.Equals(input.EventLogTag))
                ) && 
                (
                    this.EventMsgSpoolUsageThreshold == input.EventMsgSpoolUsageThreshold ||
                    (this.EventMsgSpoolUsageThreshold != null &&
                    this.EventMsgSpoolUsageThreshold.Equals(input.EventMsgSpoolUsageThreshold))
                ) && 
                (
                    this.EventPublishClientEnabled == input.EventPublishClientEnabled ||
                    (this.EventPublishClientEnabled != null &&
                    this.EventPublishClientEnabled.Equals(input.EventPublishClientEnabled))
                ) && 
                (
                    this.EventPublishMsgVpnEnabled == input.EventPublishMsgVpnEnabled ||
                    (this.EventPublishMsgVpnEnabled != null &&
                    this.EventPublishMsgVpnEnabled.Equals(input.EventPublishMsgVpnEnabled))
                ) && 
                (
                    this.EventPublishSubscriptionMode == input.EventPublishSubscriptionMode ||
                    (this.EventPublishSubscriptionMode != null &&
                    this.EventPublishSubscriptionMode.Equals(input.EventPublishSubscriptionMode))
                ) && 
                (
                    this.EventPublishTopicFormatMqttEnabled == input.EventPublishTopicFormatMqttEnabled ||
                    (this.EventPublishTopicFormatMqttEnabled != null &&
                    this.EventPublishTopicFormatMqttEnabled.Equals(input.EventPublishTopicFormatMqttEnabled))
                ) && 
                (
                    this.EventPublishTopicFormatSmfEnabled == input.EventPublishTopicFormatSmfEnabled ||
                    (this.EventPublishTopicFormatSmfEnabled != null &&
                    this.EventPublishTopicFormatSmfEnabled.Equals(input.EventPublishTopicFormatSmfEnabled))
                ) && 
                (
                    this.EventServiceAmqpConnectionCountThreshold == input.EventServiceAmqpConnectionCountThreshold ||
                    (this.EventServiceAmqpConnectionCountThreshold != null &&
                    this.EventServiceAmqpConnectionCountThreshold.Equals(input.EventServiceAmqpConnectionCountThreshold))
                ) && 
                (
                    this.EventServiceMqttConnectionCountThreshold == input.EventServiceMqttConnectionCountThreshold ||
                    (this.EventServiceMqttConnectionCountThreshold != null &&
                    this.EventServiceMqttConnectionCountThreshold.Equals(input.EventServiceMqttConnectionCountThreshold))
                ) && 
                (
                    this.EventServiceRestIncomingConnectionCountThreshold == input.EventServiceRestIncomingConnectionCountThreshold ||
                    (this.EventServiceRestIncomingConnectionCountThreshold != null &&
                    this.EventServiceRestIncomingConnectionCountThreshold.Equals(input.EventServiceRestIncomingConnectionCountThreshold))
                ) && 
                (
                    this.EventServiceSmfConnectionCountThreshold == input.EventServiceSmfConnectionCountThreshold ||
                    (this.EventServiceSmfConnectionCountThreshold != null &&
                    this.EventServiceSmfConnectionCountThreshold.Equals(input.EventServiceSmfConnectionCountThreshold))
                ) && 
                (
                    this.EventServiceWebConnectionCountThreshold == input.EventServiceWebConnectionCountThreshold ||
                    (this.EventServiceWebConnectionCountThreshold != null &&
                    this.EventServiceWebConnectionCountThreshold.Equals(input.EventServiceWebConnectionCountThreshold))
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
                    this.ExportSubscriptionsEnabled == input.ExportSubscriptionsEnabled ||
                    (this.ExportSubscriptionsEnabled != null &&
                    this.ExportSubscriptionsEnabled.Equals(input.ExportSubscriptionsEnabled))
                ) && 
                (
                    this.JndiEnabled == input.JndiEnabled ||
                    (this.JndiEnabled != null &&
                    this.JndiEnabled.Equals(input.JndiEnabled))
                ) && 
                (
                    this.MaxConnectionCount == input.MaxConnectionCount ||
                    (this.MaxConnectionCount != null &&
                    this.MaxConnectionCount.Equals(input.MaxConnectionCount))
                ) && 
                (
                    this.MaxEgressFlowCount == input.MaxEgressFlowCount ||
                    (this.MaxEgressFlowCount != null &&
                    this.MaxEgressFlowCount.Equals(input.MaxEgressFlowCount))
                ) && 
                (
                    this.MaxEndpointCount == input.MaxEndpointCount ||
                    (this.MaxEndpointCount != null &&
                    this.MaxEndpointCount.Equals(input.MaxEndpointCount))
                ) && 
                (
                    this.MaxIngressFlowCount == input.MaxIngressFlowCount ||
                    (this.MaxIngressFlowCount != null &&
                    this.MaxIngressFlowCount.Equals(input.MaxIngressFlowCount))
                ) && 
                (
                    this.MaxMsgSpoolUsage == input.MaxMsgSpoolUsage ||
                    (this.MaxMsgSpoolUsage != null &&
                    this.MaxMsgSpoolUsage.Equals(input.MaxMsgSpoolUsage))
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
                    this.MqttRetainMaxMemory == input.MqttRetainMaxMemory ||
                    (this.MqttRetainMaxMemory != null &&
                    this.MqttRetainMaxMemory.Equals(input.MqttRetainMaxMemory))
                ) && 
                (
                    this.MsgVpnName == input.MsgVpnName ||
                    (this.MsgVpnName != null &&
                    this.MsgVpnName.Equals(input.MsgVpnName))
                ) && 
                (
                    this.ReplicationAckPropagationIntervalMsgCount == input.ReplicationAckPropagationIntervalMsgCount ||
                    (this.ReplicationAckPropagationIntervalMsgCount != null &&
                    this.ReplicationAckPropagationIntervalMsgCount.Equals(input.ReplicationAckPropagationIntervalMsgCount))
                ) && 
                (
                    this.ReplicationBridgeAuthenticationBasicClientUsername == input.ReplicationBridgeAuthenticationBasicClientUsername ||
                    (this.ReplicationBridgeAuthenticationBasicClientUsername != null &&
                    this.ReplicationBridgeAuthenticationBasicClientUsername.Equals(input.ReplicationBridgeAuthenticationBasicClientUsername))
                ) && 
                (
                    this.ReplicationBridgeAuthenticationBasicPassword == input.ReplicationBridgeAuthenticationBasicPassword ||
                    (this.ReplicationBridgeAuthenticationBasicPassword != null &&
                    this.ReplicationBridgeAuthenticationBasicPassword.Equals(input.ReplicationBridgeAuthenticationBasicPassword))
                ) && 
                (
                    this.ReplicationBridgeAuthenticationClientCertContent == input.ReplicationBridgeAuthenticationClientCertContent ||
                    (this.ReplicationBridgeAuthenticationClientCertContent != null &&
                    this.ReplicationBridgeAuthenticationClientCertContent.Equals(input.ReplicationBridgeAuthenticationClientCertContent))
                ) && 
                (
                    this.ReplicationBridgeAuthenticationClientCertPassword == input.ReplicationBridgeAuthenticationClientCertPassword ||
                    (this.ReplicationBridgeAuthenticationClientCertPassword != null &&
                    this.ReplicationBridgeAuthenticationClientCertPassword.Equals(input.ReplicationBridgeAuthenticationClientCertPassword))
                ) && 
                (
                    this.ReplicationBridgeAuthenticationScheme == input.ReplicationBridgeAuthenticationScheme ||
                    (this.ReplicationBridgeAuthenticationScheme != null &&
                    this.ReplicationBridgeAuthenticationScheme.Equals(input.ReplicationBridgeAuthenticationScheme))
                ) && 
                (
                    this.ReplicationBridgeCompressedDataEnabled == input.ReplicationBridgeCompressedDataEnabled ||
                    (this.ReplicationBridgeCompressedDataEnabled != null &&
                    this.ReplicationBridgeCompressedDataEnabled.Equals(input.ReplicationBridgeCompressedDataEnabled))
                ) && 
                (
                    this.ReplicationBridgeEgressFlowWindowSize == input.ReplicationBridgeEgressFlowWindowSize ||
                    (this.ReplicationBridgeEgressFlowWindowSize != null &&
                    this.ReplicationBridgeEgressFlowWindowSize.Equals(input.ReplicationBridgeEgressFlowWindowSize))
                ) && 
                (
                    this.ReplicationBridgeRetryDelay == input.ReplicationBridgeRetryDelay ||
                    (this.ReplicationBridgeRetryDelay != null &&
                    this.ReplicationBridgeRetryDelay.Equals(input.ReplicationBridgeRetryDelay))
                ) && 
                (
                    this.ReplicationBridgeTlsEnabled == input.ReplicationBridgeTlsEnabled ||
                    (this.ReplicationBridgeTlsEnabled != null &&
                    this.ReplicationBridgeTlsEnabled.Equals(input.ReplicationBridgeTlsEnabled))
                ) && 
                (
                    this.ReplicationBridgeUnidirectionalClientProfileName == input.ReplicationBridgeUnidirectionalClientProfileName ||
                    (this.ReplicationBridgeUnidirectionalClientProfileName != null &&
                    this.ReplicationBridgeUnidirectionalClientProfileName.Equals(input.ReplicationBridgeUnidirectionalClientProfileName))
                ) && 
                (
                    this.ReplicationEnabled == input.ReplicationEnabled ||
                    (this.ReplicationEnabled != null &&
                    this.ReplicationEnabled.Equals(input.ReplicationEnabled))
                ) && 
                (
                    this.ReplicationEnabledQueueBehavior == input.ReplicationEnabledQueueBehavior ||
                    (this.ReplicationEnabledQueueBehavior != null &&
                    this.ReplicationEnabledQueueBehavior.Equals(input.ReplicationEnabledQueueBehavior))
                ) && 
                (
                    this.ReplicationQueueMaxMsgSpoolUsage == input.ReplicationQueueMaxMsgSpoolUsage ||
                    (this.ReplicationQueueMaxMsgSpoolUsage != null &&
                    this.ReplicationQueueMaxMsgSpoolUsage.Equals(input.ReplicationQueueMaxMsgSpoolUsage))
                ) && 
                (
                    this.ReplicationQueueRejectMsgToSenderOnDiscardEnabled == input.ReplicationQueueRejectMsgToSenderOnDiscardEnabled ||
                    (this.ReplicationQueueRejectMsgToSenderOnDiscardEnabled != null &&
                    this.ReplicationQueueRejectMsgToSenderOnDiscardEnabled.Equals(input.ReplicationQueueRejectMsgToSenderOnDiscardEnabled))
                ) && 
                (
                    this.ReplicationRejectMsgWhenSyncIneligibleEnabled == input.ReplicationRejectMsgWhenSyncIneligibleEnabled ||
                    (this.ReplicationRejectMsgWhenSyncIneligibleEnabled != null &&
                    this.ReplicationRejectMsgWhenSyncIneligibleEnabled.Equals(input.ReplicationRejectMsgWhenSyncIneligibleEnabled))
                ) && 
                (
                    this.ReplicationRole == input.ReplicationRole ||
                    (this.ReplicationRole != null &&
                    this.ReplicationRole.Equals(input.ReplicationRole))
                ) && 
                (
                    this.ReplicationTransactionMode == input.ReplicationTransactionMode ||
                    (this.ReplicationTransactionMode != null &&
                    this.ReplicationTransactionMode.Equals(input.ReplicationTransactionMode))
                ) && 
                (
                    this.RestTlsServerCertEnforceTrustedCommonNameEnabled == input.RestTlsServerCertEnforceTrustedCommonNameEnabled ||
                    (this.RestTlsServerCertEnforceTrustedCommonNameEnabled != null &&
                    this.RestTlsServerCertEnforceTrustedCommonNameEnabled.Equals(input.RestTlsServerCertEnforceTrustedCommonNameEnabled))
                ) && 
                (
                    this.RestTlsServerCertMaxChainDepth == input.RestTlsServerCertMaxChainDepth ||
                    (this.RestTlsServerCertMaxChainDepth != null &&
                    this.RestTlsServerCertMaxChainDepth.Equals(input.RestTlsServerCertMaxChainDepth))
                ) && 
                (
                    this.RestTlsServerCertValidateDateEnabled == input.RestTlsServerCertValidateDateEnabled ||
                    (this.RestTlsServerCertValidateDateEnabled != null &&
                    this.RestTlsServerCertValidateDateEnabled.Equals(input.RestTlsServerCertValidateDateEnabled))
                ) && 
                (
                    this.RestTlsServerCertValidateNameEnabled == input.RestTlsServerCertValidateNameEnabled ||
                    (this.RestTlsServerCertValidateNameEnabled != null &&
                    this.RestTlsServerCertValidateNameEnabled.Equals(input.RestTlsServerCertValidateNameEnabled))
                ) && 
                (
                    this.SempOverMsgBusAdminClientEnabled == input.SempOverMsgBusAdminClientEnabled ||
                    (this.SempOverMsgBusAdminClientEnabled != null &&
                    this.SempOverMsgBusAdminClientEnabled.Equals(input.SempOverMsgBusAdminClientEnabled))
                ) && 
                (
                    this.SempOverMsgBusAdminDistributedCacheEnabled == input.SempOverMsgBusAdminDistributedCacheEnabled ||
                    (this.SempOverMsgBusAdminDistributedCacheEnabled != null &&
                    this.SempOverMsgBusAdminDistributedCacheEnabled.Equals(input.SempOverMsgBusAdminDistributedCacheEnabled))
                ) && 
                (
                    this.SempOverMsgBusAdminEnabled == input.SempOverMsgBusAdminEnabled ||
                    (this.SempOverMsgBusAdminEnabled != null &&
                    this.SempOverMsgBusAdminEnabled.Equals(input.SempOverMsgBusAdminEnabled))
                ) && 
                (
                    this.SempOverMsgBusEnabled == input.SempOverMsgBusEnabled ||
                    (this.SempOverMsgBusEnabled != null &&
                    this.SempOverMsgBusEnabled.Equals(input.SempOverMsgBusEnabled))
                ) && 
                (
                    this.SempOverMsgBusShowEnabled == input.SempOverMsgBusShowEnabled ||
                    (this.SempOverMsgBusShowEnabled != null &&
                    this.SempOverMsgBusShowEnabled.Equals(input.SempOverMsgBusShowEnabled))
                ) && 
                (
                    this.ServiceAmqpMaxConnectionCount == input.ServiceAmqpMaxConnectionCount ||
                    (this.ServiceAmqpMaxConnectionCount != null &&
                    this.ServiceAmqpMaxConnectionCount.Equals(input.ServiceAmqpMaxConnectionCount))
                ) && 
                (
                    this.ServiceAmqpPlainTextEnabled == input.ServiceAmqpPlainTextEnabled ||
                    (this.ServiceAmqpPlainTextEnabled != null &&
                    this.ServiceAmqpPlainTextEnabled.Equals(input.ServiceAmqpPlainTextEnabled))
                ) && 
                (
                    this.ServiceAmqpPlainTextListenPort == input.ServiceAmqpPlainTextListenPort ||
                    (this.ServiceAmqpPlainTextListenPort != null &&
                    this.ServiceAmqpPlainTextListenPort.Equals(input.ServiceAmqpPlainTextListenPort))
                ) && 
                (
                    this.ServiceAmqpTlsEnabled == input.ServiceAmqpTlsEnabled ||
                    (this.ServiceAmqpTlsEnabled != null &&
                    this.ServiceAmqpTlsEnabled.Equals(input.ServiceAmqpTlsEnabled))
                ) && 
                (
                    this.ServiceAmqpTlsListenPort == input.ServiceAmqpTlsListenPort ||
                    (this.ServiceAmqpTlsListenPort != null &&
                    this.ServiceAmqpTlsListenPort.Equals(input.ServiceAmqpTlsListenPort))
                ) && 
                (
                    this.ServiceMqttAuthenticationClientCertRequest == input.ServiceMqttAuthenticationClientCertRequest ||
                    (this.ServiceMqttAuthenticationClientCertRequest != null &&
                    this.ServiceMqttAuthenticationClientCertRequest.Equals(input.ServiceMqttAuthenticationClientCertRequest))
                ) && 
                (
                    this.ServiceMqttMaxConnectionCount == input.ServiceMqttMaxConnectionCount ||
                    (this.ServiceMqttMaxConnectionCount != null &&
                    this.ServiceMqttMaxConnectionCount.Equals(input.ServiceMqttMaxConnectionCount))
                ) && 
                (
                    this.ServiceMqttPlainTextEnabled == input.ServiceMqttPlainTextEnabled ||
                    (this.ServiceMqttPlainTextEnabled != null &&
                    this.ServiceMqttPlainTextEnabled.Equals(input.ServiceMqttPlainTextEnabled))
                ) && 
                (
                    this.ServiceMqttPlainTextListenPort == input.ServiceMqttPlainTextListenPort ||
                    (this.ServiceMqttPlainTextListenPort != null &&
                    this.ServiceMqttPlainTextListenPort.Equals(input.ServiceMqttPlainTextListenPort))
                ) && 
                (
                    this.ServiceMqttTlsEnabled == input.ServiceMqttTlsEnabled ||
                    (this.ServiceMqttTlsEnabled != null &&
                    this.ServiceMqttTlsEnabled.Equals(input.ServiceMqttTlsEnabled))
                ) && 
                (
                    this.ServiceMqttTlsListenPort == input.ServiceMqttTlsListenPort ||
                    (this.ServiceMqttTlsListenPort != null &&
                    this.ServiceMqttTlsListenPort.Equals(input.ServiceMqttTlsListenPort))
                ) && 
                (
                    this.ServiceMqttTlsWebSocketEnabled == input.ServiceMqttTlsWebSocketEnabled ||
                    (this.ServiceMqttTlsWebSocketEnabled != null &&
                    this.ServiceMqttTlsWebSocketEnabled.Equals(input.ServiceMqttTlsWebSocketEnabled))
                ) && 
                (
                    this.ServiceMqttTlsWebSocketListenPort == input.ServiceMqttTlsWebSocketListenPort ||
                    (this.ServiceMqttTlsWebSocketListenPort != null &&
                    this.ServiceMqttTlsWebSocketListenPort.Equals(input.ServiceMqttTlsWebSocketListenPort))
                ) && 
                (
                    this.ServiceMqttWebSocketEnabled == input.ServiceMqttWebSocketEnabled ||
                    (this.ServiceMqttWebSocketEnabled != null &&
                    this.ServiceMqttWebSocketEnabled.Equals(input.ServiceMqttWebSocketEnabled))
                ) && 
                (
                    this.ServiceMqttWebSocketListenPort == input.ServiceMqttWebSocketListenPort ||
                    (this.ServiceMqttWebSocketListenPort != null &&
                    this.ServiceMqttWebSocketListenPort.Equals(input.ServiceMqttWebSocketListenPort))
                ) && 
                (
                    this.ServiceRestIncomingAuthenticationClientCertRequest == input.ServiceRestIncomingAuthenticationClientCertRequest ||
                    (this.ServiceRestIncomingAuthenticationClientCertRequest != null &&
                    this.ServiceRestIncomingAuthenticationClientCertRequest.Equals(input.ServiceRestIncomingAuthenticationClientCertRequest))
                ) && 
                (
                    this.ServiceRestIncomingAuthorizationHeaderHandling == input.ServiceRestIncomingAuthorizationHeaderHandling ||
                    (this.ServiceRestIncomingAuthorizationHeaderHandling != null &&
                    this.ServiceRestIncomingAuthorizationHeaderHandling.Equals(input.ServiceRestIncomingAuthorizationHeaderHandling))
                ) && 
                (
                    this.ServiceRestIncomingMaxConnectionCount == input.ServiceRestIncomingMaxConnectionCount ||
                    (this.ServiceRestIncomingMaxConnectionCount != null &&
                    this.ServiceRestIncomingMaxConnectionCount.Equals(input.ServiceRestIncomingMaxConnectionCount))
                ) && 
                (
                    this.ServiceRestIncomingPlainTextEnabled == input.ServiceRestIncomingPlainTextEnabled ||
                    (this.ServiceRestIncomingPlainTextEnabled != null &&
                    this.ServiceRestIncomingPlainTextEnabled.Equals(input.ServiceRestIncomingPlainTextEnabled))
                ) && 
                (
                    this.ServiceRestIncomingPlainTextListenPort == input.ServiceRestIncomingPlainTextListenPort ||
                    (this.ServiceRestIncomingPlainTextListenPort != null &&
                    this.ServiceRestIncomingPlainTextListenPort.Equals(input.ServiceRestIncomingPlainTextListenPort))
                ) && 
                (
                    this.ServiceRestIncomingTlsEnabled == input.ServiceRestIncomingTlsEnabled ||
                    (this.ServiceRestIncomingTlsEnabled != null &&
                    this.ServiceRestIncomingTlsEnabled.Equals(input.ServiceRestIncomingTlsEnabled))
                ) && 
                (
                    this.ServiceRestIncomingTlsListenPort == input.ServiceRestIncomingTlsListenPort ||
                    (this.ServiceRestIncomingTlsListenPort != null &&
                    this.ServiceRestIncomingTlsListenPort.Equals(input.ServiceRestIncomingTlsListenPort))
                ) && 
                (
                    this.ServiceRestMode == input.ServiceRestMode ||
                    (this.ServiceRestMode != null &&
                    this.ServiceRestMode.Equals(input.ServiceRestMode))
                ) && 
                (
                    this.ServiceRestOutgoingMaxConnectionCount == input.ServiceRestOutgoingMaxConnectionCount ||
                    (this.ServiceRestOutgoingMaxConnectionCount != null &&
                    this.ServiceRestOutgoingMaxConnectionCount.Equals(input.ServiceRestOutgoingMaxConnectionCount))
                ) && 
                (
                    this.ServiceSmfMaxConnectionCount == input.ServiceSmfMaxConnectionCount ||
                    (this.ServiceSmfMaxConnectionCount != null &&
                    this.ServiceSmfMaxConnectionCount.Equals(input.ServiceSmfMaxConnectionCount))
                ) && 
                (
                    this.ServiceSmfPlainTextEnabled == input.ServiceSmfPlainTextEnabled ||
                    (this.ServiceSmfPlainTextEnabled != null &&
                    this.ServiceSmfPlainTextEnabled.Equals(input.ServiceSmfPlainTextEnabled))
                ) && 
                (
                    this.ServiceSmfTlsEnabled == input.ServiceSmfTlsEnabled ||
                    (this.ServiceSmfTlsEnabled != null &&
                    this.ServiceSmfTlsEnabled.Equals(input.ServiceSmfTlsEnabled))
                ) && 
                (
                    this.ServiceWebAuthenticationClientCertRequest == input.ServiceWebAuthenticationClientCertRequest ||
                    (this.ServiceWebAuthenticationClientCertRequest != null &&
                    this.ServiceWebAuthenticationClientCertRequest.Equals(input.ServiceWebAuthenticationClientCertRequest))
                ) && 
                (
                    this.ServiceWebMaxConnectionCount == input.ServiceWebMaxConnectionCount ||
                    (this.ServiceWebMaxConnectionCount != null &&
                    this.ServiceWebMaxConnectionCount.Equals(input.ServiceWebMaxConnectionCount))
                ) && 
                (
                    this.ServiceWebPlainTextEnabled == input.ServiceWebPlainTextEnabled ||
                    (this.ServiceWebPlainTextEnabled != null &&
                    this.ServiceWebPlainTextEnabled.Equals(input.ServiceWebPlainTextEnabled))
                ) && 
                (
                    this.ServiceWebTlsEnabled == input.ServiceWebTlsEnabled ||
                    (this.ServiceWebTlsEnabled != null &&
                    this.ServiceWebTlsEnabled.Equals(input.ServiceWebTlsEnabled))
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
                if (this.Alias != null)
                    hashCode = hashCode * 59 + this.Alias.GetHashCode();
                if (this.AuthenticationBasicEnabled != null)
                    hashCode = hashCode * 59 + this.AuthenticationBasicEnabled.GetHashCode();
                if (this.AuthenticationBasicProfileName != null)
                    hashCode = hashCode * 59 + this.AuthenticationBasicProfileName.GetHashCode();
                if (this.AuthenticationBasicRadiusDomain != null)
                    hashCode = hashCode * 59 + this.AuthenticationBasicRadiusDomain.GetHashCode();
                if (this.AuthenticationBasicType != null)
                    hashCode = hashCode * 59 + this.AuthenticationBasicType.GetHashCode();
                if (this.AuthenticationClientCertAllowApiProvidedUsernameEnabled != null)
                    hashCode = hashCode * 59 + this.AuthenticationClientCertAllowApiProvidedUsernameEnabled.GetHashCode();
                if (this.AuthenticationClientCertCertificateMatchingRulesEnabled != null)
                    hashCode = hashCode * 59 + this.AuthenticationClientCertCertificateMatchingRulesEnabled.GetHashCode();
                if (this.AuthenticationClientCertEnabled != null)
                    hashCode = hashCode * 59 + this.AuthenticationClientCertEnabled.GetHashCode();
                if (this.AuthenticationClientCertMaxChainDepth != null)
                    hashCode = hashCode * 59 + this.AuthenticationClientCertMaxChainDepth.GetHashCode();
                if (this.AuthenticationClientCertRevocationCheckMode != null)
                    hashCode = hashCode * 59 + this.AuthenticationClientCertRevocationCheckMode.GetHashCode();
                if (this.AuthenticationClientCertUsernameSource != null)
                    hashCode = hashCode * 59 + this.AuthenticationClientCertUsernameSource.GetHashCode();
                if (this.AuthenticationClientCertValidateDateEnabled != null)
                    hashCode = hashCode * 59 + this.AuthenticationClientCertValidateDateEnabled.GetHashCode();
                if (this.AuthenticationKerberosAllowApiProvidedUsernameEnabled != null)
                    hashCode = hashCode * 59 + this.AuthenticationKerberosAllowApiProvidedUsernameEnabled.GetHashCode();
                if (this.AuthenticationKerberosEnabled != null)
                    hashCode = hashCode * 59 + this.AuthenticationKerberosEnabled.GetHashCode();
                if (this.AuthenticationOauthDefaultProfileName != null)
                    hashCode = hashCode * 59 + this.AuthenticationOauthDefaultProfileName.GetHashCode();
                if (this.AuthenticationOauthDefaultProviderName != null)
                    hashCode = hashCode * 59 + this.AuthenticationOauthDefaultProviderName.GetHashCode();
                if (this.AuthenticationOauthEnabled != null)
                    hashCode = hashCode * 59 + this.AuthenticationOauthEnabled.GetHashCode();
                if (this.AuthorizationLdapGroupMembershipAttributeName != null)
                    hashCode = hashCode * 59 + this.AuthorizationLdapGroupMembershipAttributeName.GetHashCode();
                if (this.AuthorizationLdapTrimClientUsernameDomainEnabled != null)
                    hashCode = hashCode * 59 + this.AuthorizationLdapTrimClientUsernameDomainEnabled.GetHashCode();
                if (this.AuthorizationProfileName != null)
                    hashCode = hashCode * 59 + this.AuthorizationProfileName.GetHashCode();
                if (this.AuthorizationType != null)
                    hashCode = hashCode * 59 + this.AuthorizationType.GetHashCode();
                if (this.BridgingTlsServerCertEnforceTrustedCommonNameEnabled != null)
                    hashCode = hashCode * 59 + this.BridgingTlsServerCertEnforceTrustedCommonNameEnabled.GetHashCode();
                if (this.BridgingTlsServerCertMaxChainDepth != null)
                    hashCode = hashCode * 59 + this.BridgingTlsServerCertMaxChainDepth.GetHashCode();
                if (this.BridgingTlsServerCertValidateDateEnabled != null)
                    hashCode = hashCode * 59 + this.BridgingTlsServerCertValidateDateEnabled.GetHashCode();
                if (this.BridgingTlsServerCertValidateNameEnabled != null)
                    hashCode = hashCode * 59 + this.BridgingTlsServerCertValidateNameEnabled.GetHashCode();
                if (this.DistributedCacheManagementEnabled != null)
                    hashCode = hashCode * 59 + this.DistributedCacheManagementEnabled.GetHashCode();
                if (this.DmrEnabled != null)
                    hashCode = hashCode * 59 + this.DmrEnabled.GetHashCode();
                if (this.Enabled != null)
                    hashCode = hashCode * 59 + this.Enabled.GetHashCode();
                if (this.EventConnectionCountThreshold != null)
                    hashCode = hashCode * 59 + this.EventConnectionCountThreshold.GetHashCode();
                if (this.EventEgressFlowCountThreshold != null)
                    hashCode = hashCode * 59 + this.EventEgressFlowCountThreshold.GetHashCode();
                if (this.EventEgressMsgRateThreshold != null)
                    hashCode = hashCode * 59 + this.EventEgressMsgRateThreshold.GetHashCode();
                if (this.EventEndpointCountThreshold != null)
                    hashCode = hashCode * 59 + this.EventEndpointCountThreshold.GetHashCode();
                if (this.EventIngressFlowCountThreshold != null)
                    hashCode = hashCode * 59 + this.EventIngressFlowCountThreshold.GetHashCode();
                if (this.EventIngressMsgRateThreshold != null)
                    hashCode = hashCode * 59 + this.EventIngressMsgRateThreshold.GetHashCode();
                if (this.EventLargeMsgThreshold != null)
                    hashCode = hashCode * 59 + this.EventLargeMsgThreshold.GetHashCode();
                if (this.EventLogTag != null)
                    hashCode = hashCode * 59 + this.EventLogTag.GetHashCode();
                if (this.EventMsgSpoolUsageThreshold != null)
                    hashCode = hashCode * 59 + this.EventMsgSpoolUsageThreshold.GetHashCode();
                if (this.EventPublishClientEnabled != null)
                    hashCode = hashCode * 59 + this.EventPublishClientEnabled.GetHashCode();
                if (this.EventPublishMsgVpnEnabled != null)
                    hashCode = hashCode * 59 + this.EventPublishMsgVpnEnabled.GetHashCode();
                if (this.EventPublishSubscriptionMode != null)
                    hashCode = hashCode * 59 + this.EventPublishSubscriptionMode.GetHashCode();
                if (this.EventPublishTopicFormatMqttEnabled != null)
                    hashCode = hashCode * 59 + this.EventPublishTopicFormatMqttEnabled.GetHashCode();
                if (this.EventPublishTopicFormatSmfEnabled != null)
                    hashCode = hashCode * 59 + this.EventPublishTopicFormatSmfEnabled.GetHashCode();
                if (this.EventServiceAmqpConnectionCountThreshold != null)
                    hashCode = hashCode * 59 + this.EventServiceAmqpConnectionCountThreshold.GetHashCode();
                if (this.EventServiceMqttConnectionCountThreshold != null)
                    hashCode = hashCode * 59 + this.EventServiceMqttConnectionCountThreshold.GetHashCode();
                if (this.EventServiceRestIncomingConnectionCountThreshold != null)
                    hashCode = hashCode * 59 + this.EventServiceRestIncomingConnectionCountThreshold.GetHashCode();
                if (this.EventServiceSmfConnectionCountThreshold != null)
                    hashCode = hashCode * 59 + this.EventServiceSmfConnectionCountThreshold.GetHashCode();
                if (this.EventServiceWebConnectionCountThreshold != null)
                    hashCode = hashCode * 59 + this.EventServiceWebConnectionCountThreshold.GetHashCode();
                if (this.EventSubscriptionCountThreshold != null)
                    hashCode = hashCode * 59 + this.EventSubscriptionCountThreshold.GetHashCode();
                if (this.EventTransactedSessionCountThreshold != null)
                    hashCode = hashCode * 59 + this.EventTransactedSessionCountThreshold.GetHashCode();
                if (this.EventTransactionCountThreshold != null)
                    hashCode = hashCode * 59 + this.EventTransactionCountThreshold.GetHashCode();
                if (this.ExportSubscriptionsEnabled != null)
                    hashCode = hashCode * 59 + this.ExportSubscriptionsEnabled.GetHashCode();
                if (this.JndiEnabled != null)
                    hashCode = hashCode * 59 + this.JndiEnabled.GetHashCode();
                if (this.MaxConnectionCount != null)
                    hashCode = hashCode * 59 + this.MaxConnectionCount.GetHashCode();
                if (this.MaxEgressFlowCount != null)
                    hashCode = hashCode * 59 + this.MaxEgressFlowCount.GetHashCode();
                if (this.MaxEndpointCount != null)
                    hashCode = hashCode * 59 + this.MaxEndpointCount.GetHashCode();
                if (this.MaxIngressFlowCount != null)
                    hashCode = hashCode * 59 + this.MaxIngressFlowCount.GetHashCode();
                if (this.MaxMsgSpoolUsage != null)
                    hashCode = hashCode * 59 + this.MaxMsgSpoolUsage.GetHashCode();
                if (this.MaxSubscriptionCount != null)
                    hashCode = hashCode * 59 + this.MaxSubscriptionCount.GetHashCode();
                if (this.MaxTransactedSessionCount != null)
                    hashCode = hashCode * 59 + this.MaxTransactedSessionCount.GetHashCode();
                if (this.MaxTransactionCount != null)
                    hashCode = hashCode * 59 + this.MaxTransactionCount.GetHashCode();
                if (this.MqttRetainMaxMemory != null)
                    hashCode = hashCode * 59 + this.MqttRetainMaxMemory.GetHashCode();
                if (this.MsgVpnName != null)
                    hashCode = hashCode * 59 + this.MsgVpnName.GetHashCode();
                if (this.ReplicationAckPropagationIntervalMsgCount != null)
                    hashCode = hashCode * 59 + this.ReplicationAckPropagationIntervalMsgCount.GetHashCode();
                if (this.ReplicationBridgeAuthenticationBasicClientUsername != null)
                    hashCode = hashCode * 59 + this.ReplicationBridgeAuthenticationBasicClientUsername.GetHashCode();
                if (this.ReplicationBridgeAuthenticationBasicPassword != null)
                    hashCode = hashCode * 59 + this.ReplicationBridgeAuthenticationBasicPassword.GetHashCode();
                if (this.ReplicationBridgeAuthenticationClientCertContent != null)
                    hashCode = hashCode * 59 + this.ReplicationBridgeAuthenticationClientCertContent.GetHashCode();
                if (this.ReplicationBridgeAuthenticationClientCertPassword != null)
                    hashCode = hashCode * 59 + this.ReplicationBridgeAuthenticationClientCertPassword.GetHashCode();
                if (this.ReplicationBridgeAuthenticationScheme != null)
                    hashCode = hashCode * 59 + this.ReplicationBridgeAuthenticationScheme.GetHashCode();
                if (this.ReplicationBridgeCompressedDataEnabled != null)
                    hashCode = hashCode * 59 + this.ReplicationBridgeCompressedDataEnabled.GetHashCode();
                if (this.ReplicationBridgeEgressFlowWindowSize != null)
                    hashCode = hashCode * 59 + this.ReplicationBridgeEgressFlowWindowSize.GetHashCode();
                if (this.ReplicationBridgeRetryDelay != null)
                    hashCode = hashCode * 59 + this.ReplicationBridgeRetryDelay.GetHashCode();
                if (this.ReplicationBridgeTlsEnabled != null)
                    hashCode = hashCode * 59 + this.ReplicationBridgeTlsEnabled.GetHashCode();
                if (this.ReplicationBridgeUnidirectionalClientProfileName != null)
                    hashCode = hashCode * 59 + this.ReplicationBridgeUnidirectionalClientProfileName.GetHashCode();
                if (this.ReplicationEnabled != null)
                    hashCode = hashCode * 59 + this.ReplicationEnabled.GetHashCode();
                if (this.ReplicationEnabledQueueBehavior != null)
                    hashCode = hashCode * 59 + this.ReplicationEnabledQueueBehavior.GetHashCode();
                if (this.ReplicationQueueMaxMsgSpoolUsage != null)
                    hashCode = hashCode * 59 + this.ReplicationQueueMaxMsgSpoolUsage.GetHashCode();
                if (this.ReplicationQueueRejectMsgToSenderOnDiscardEnabled != null)
                    hashCode = hashCode * 59 + this.ReplicationQueueRejectMsgToSenderOnDiscardEnabled.GetHashCode();
                if (this.ReplicationRejectMsgWhenSyncIneligibleEnabled != null)
                    hashCode = hashCode * 59 + this.ReplicationRejectMsgWhenSyncIneligibleEnabled.GetHashCode();
                if (this.ReplicationRole != null)
                    hashCode = hashCode * 59 + this.ReplicationRole.GetHashCode();
                if (this.ReplicationTransactionMode != null)
                    hashCode = hashCode * 59 + this.ReplicationTransactionMode.GetHashCode();
                if (this.RestTlsServerCertEnforceTrustedCommonNameEnabled != null)
                    hashCode = hashCode * 59 + this.RestTlsServerCertEnforceTrustedCommonNameEnabled.GetHashCode();
                if (this.RestTlsServerCertMaxChainDepth != null)
                    hashCode = hashCode * 59 + this.RestTlsServerCertMaxChainDepth.GetHashCode();
                if (this.RestTlsServerCertValidateDateEnabled != null)
                    hashCode = hashCode * 59 + this.RestTlsServerCertValidateDateEnabled.GetHashCode();
                if (this.RestTlsServerCertValidateNameEnabled != null)
                    hashCode = hashCode * 59 + this.RestTlsServerCertValidateNameEnabled.GetHashCode();
                if (this.SempOverMsgBusAdminClientEnabled != null)
                    hashCode = hashCode * 59 + this.SempOverMsgBusAdminClientEnabled.GetHashCode();
                if (this.SempOverMsgBusAdminDistributedCacheEnabled != null)
                    hashCode = hashCode * 59 + this.SempOverMsgBusAdminDistributedCacheEnabled.GetHashCode();
                if (this.SempOverMsgBusAdminEnabled != null)
                    hashCode = hashCode * 59 + this.SempOverMsgBusAdminEnabled.GetHashCode();
                if (this.SempOverMsgBusEnabled != null)
                    hashCode = hashCode * 59 + this.SempOverMsgBusEnabled.GetHashCode();
                if (this.SempOverMsgBusShowEnabled != null)
                    hashCode = hashCode * 59 + this.SempOverMsgBusShowEnabled.GetHashCode();
                if (this.ServiceAmqpMaxConnectionCount != null)
                    hashCode = hashCode * 59 + this.ServiceAmqpMaxConnectionCount.GetHashCode();
                if (this.ServiceAmqpPlainTextEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceAmqpPlainTextEnabled.GetHashCode();
                if (this.ServiceAmqpPlainTextListenPort != null)
                    hashCode = hashCode * 59 + this.ServiceAmqpPlainTextListenPort.GetHashCode();
                if (this.ServiceAmqpTlsEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceAmqpTlsEnabled.GetHashCode();
                if (this.ServiceAmqpTlsListenPort != null)
                    hashCode = hashCode * 59 + this.ServiceAmqpTlsListenPort.GetHashCode();
                if (this.ServiceMqttAuthenticationClientCertRequest != null)
                    hashCode = hashCode * 59 + this.ServiceMqttAuthenticationClientCertRequest.GetHashCode();
                if (this.ServiceMqttMaxConnectionCount != null)
                    hashCode = hashCode * 59 + this.ServiceMqttMaxConnectionCount.GetHashCode();
                if (this.ServiceMqttPlainTextEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceMqttPlainTextEnabled.GetHashCode();
                if (this.ServiceMqttPlainTextListenPort != null)
                    hashCode = hashCode * 59 + this.ServiceMqttPlainTextListenPort.GetHashCode();
                if (this.ServiceMqttTlsEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceMqttTlsEnabled.GetHashCode();
                if (this.ServiceMqttTlsListenPort != null)
                    hashCode = hashCode * 59 + this.ServiceMqttTlsListenPort.GetHashCode();
                if (this.ServiceMqttTlsWebSocketEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceMqttTlsWebSocketEnabled.GetHashCode();
                if (this.ServiceMqttTlsWebSocketListenPort != null)
                    hashCode = hashCode * 59 + this.ServiceMqttTlsWebSocketListenPort.GetHashCode();
                if (this.ServiceMqttWebSocketEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceMqttWebSocketEnabled.GetHashCode();
                if (this.ServiceMqttWebSocketListenPort != null)
                    hashCode = hashCode * 59 + this.ServiceMqttWebSocketListenPort.GetHashCode();
                if (this.ServiceRestIncomingAuthenticationClientCertRequest != null)
                    hashCode = hashCode * 59 + this.ServiceRestIncomingAuthenticationClientCertRequest.GetHashCode();
                if (this.ServiceRestIncomingAuthorizationHeaderHandling != null)
                    hashCode = hashCode * 59 + this.ServiceRestIncomingAuthorizationHeaderHandling.GetHashCode();
                if (this.ServiceRestIncomingMaxConnectionCount != null)
                    hashCode = hashCode * 59 + this.ServiceRestIncomingMaxConnectionCount.GetHashCode();
                if (this.ServiceRestIncomingPlainTextEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceRestIncomingPlainTextEnabled.GetHashCode();
                if (this.ServiceRestIncomingPlainTextListenPort != null)
                    hashCode = hashCode * 59 + this.ServiceRestIncomingPlainTextListenPort.GetHashCode();
                if (this.ServiceRestIncomingTlsEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceRestIncomingTlsEnabled.GetHashCode();
                if (this.ServiceRestIncomingTlsListenPort != null)
                    hashCode = hashCode * 59 + this.ServiceRestIncomingTlsListenPort.GetHashCode();
                if (this.ServiceRestMode != null)
                    hashCode = hashCode * 59 + this.ServiceRestMode.GetHashCode();
                if (this.ServiceRestOutgoingMaxConnectionCount != null)
                    hashCode = hashCode * 59 + this.ServiceRestOutgoingMaxConnectionCount.GetHashCode();
                if (this.ServiceSmfMaxConnectionCount != null)
                    hashCode = hashCode * 59 + this.ServiceSmfMaxConnectionCount.GetHashCode();
                if (this.ServiceSmfPlainTextEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceSmfPlainTextEnabled.GetHashCode();
                if (this.ServiceSmfTlsEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceSmfTlsEnabled.GetHashCode();
                if (this.ServiceWebAuthenticationClientCertRequest != null)
                    hashCode = hashCode * 59 + this.ServiceWebAuthenticationClientCertRequest.GetHashCode();
                if (this.ServiceWebMaxConnectionCount != null)
                    hashCode = hashCode * 59 + this.ServiceWebMaxConnectionCount.GetHashCode();
                if (this.ServiceWebPlainTextEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceWebPlainTextEnabled.GetHashCode();
                if (this.ServiceWebTlsEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceWebTlsEnabled.GetHashCode();
                if (this.TlsAllowDowngradeToPlainTextEnabled != null)
                    hashCode = hashCode * 59 + this.TlsAllowDowngradeToPlainTextEnabled.GetHashCode();
                return hashCode;
            }
        }
    }
}
