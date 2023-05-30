# Semp.V2.CSharp.Model.MsgVpnAuthorizationGroup
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**AclProfileName** | **string** | The ACL Profile of the Authorization Group. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;. | [optional] 
**AuthorizationGroupName** | **string** | The name of the Authorization Group. For LDAP groups, special care is needed if the group name contains special characters such as &#x27;#&#x27;, &#x27;+&#x27;, &#x27;;&#x27;, &#x27;&#x3D;&#x27; as the value of the group name returned from the LDAP server might prepend those characters with &#x27;\\&#x27;. For example a group name called &#x27;test#,lab,com&#x27; will be returned from the LDAP server as &#x27;test\\#,lab,com&#x27;. | [optional] 
**ClientProfileName** | **string** | The Client Profile of the Authorization Group. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;. | [optional] 
**Enabled** | **bool?** | Enable or disable the Authorization Group in the Message VPN. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. | [optional] 
**MsgVpnName** | **string** | The name of the Message VPN. | [optional] 
**OrderAfterAuthorizationGroupName** | **string** | Lower the priority to be less than this group. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is not applicable. | [optional] 
**OrderBeforeAuthorizationGroupName** | **string** | Raise the priority to be greater than this group. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is not applicable. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

