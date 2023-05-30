# Semp.V2.CSharp.Model.MsgVpnRestDeliveryPoint
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**ClientProfileName** | **string** | The Client Profile of the REST Delivery Point. It must exist in the local Message VPN. Its TCP parameters are used for all REST Consumers in this RDP. Its queue properties are used by the RDP client. The Client Profile is used inside the auto-generated Client Username for this RDP. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;. | [optional] 
**Enabled** | **bool?** | Enable or disable the REST Delivery Point. When disabled, no connections are initiated or messages delivered to any of the contained REST Consumers. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. | [optional] 
**MsgVpnName** | **string** | The name of the Message VPN. | [optional] 
**RestDeliveryPointName** | **string** | The name of the REST Delivery Point. | [optional] 
**Service** | **string** | The name of the service that this REST Delivery Point connects to. Internally the broker does not use this value; it is informational only. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.19. | [optional] 
**Vendor** | **string** | The name of the vendor that this REST Delivery Point connects to. Internally the broker does not use this value; it is informational only. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.19. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

