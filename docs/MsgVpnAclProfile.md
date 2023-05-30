# Semp.V2.CSharp.Model.MsgVpnAclProfile
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**AclProfileName** | **string** | The name of the ACL Profile. | [optional] 
**ClientConnectDefaultAction** | **string** | The default action to take when a client using the ACL Profile connects to the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;disallow\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;allow\&quot; - Allow client connection unless an exception is found for it. \&quot;disallow\&quot; - Disallow client connection unless an exception is found for it. &lt;/pre&gt;  | [optional] 
**MsgVpnName** | **string** | The name of the Message VPN. | [optional] 
**PublishTopicDefaultAction** | **string** | The default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;disallow\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;allow\&quot; - Allow topic unless an exception is found for it. \&quot;disallow\&quot; - Disallow topic unless an exception is found for it. &lt;/pre&gt;  | [optional] 
**SubscribeShareNameDefaultAction** | **string** | The default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;allow\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;allow\&quot; - Allow topic unless an exception is found for it. \&quot;disallow\&quot; - Disallow topic unless an exception is found for it. &lt;/pre&gt;  Available since 2.14. | [optional] 
**SubscribeTopicDefaultAction** | **string** | The default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;disallow\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;allow\&quot; - Allow topic unless an exception is found for it. \&quot;disallow\&quot; - Disallow topic unless an exception is found for it. &lt;/pre&gt;  | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

