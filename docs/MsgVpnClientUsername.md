# Semp.V2.CSharp.Model.MsgVpnClientUsername
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**AclProfileName** | **string** | The ACL Profile of the Client Username. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;. | [optional] 
**ClientProfileName** | **string** | The Client Profile of the Client Username. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;. | [optional] 
**ClientUsername** | **string** | The name of the Client Username. | [optional] 
**Enabled** | **bool?** | Enable or disable the Client Username. When disabled, all clients currently connected as the Client Username are disconnected. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. | [optional] 
**GuaranteedEndpointPermissionOverrideEnabled** | **bool?** | Enable or disable guaranteed endpoint permission override for the Client Username. When enabled all guaranteed endpoints may be accessed, modified or deleted with the same permission as the owner. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. | [optional] 
**MsgVpnName** | **string** | The name of the Message VPN. | [optional] 
**Password** | **string** | The password for the Client Username. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. | [optional] 
**SubscriptionManagerEnabled** | **bool?** | Enable or disable the subscription management capability of the Client Username. This is the ability to manage subscriptions on behalf of other Client Usernames. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

