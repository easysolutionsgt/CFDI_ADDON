'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports B1WizardBase
Imports SAPbobsCOM
Imports SAPbouiCOM

Namespace Factura_Electronica
    
    Public Class Button__ftTraslados01__6
        Inherits B1Item
        
        Public Sub New()
            MyBase.New
            FormType = "ftTraslados01"
            ItemUID = "6"
        End Sub

        <B1Listener(BoEventTypes.et_CLICK, False)>
        Public Overridable Sub OnAfterClick(ByVal pVal As ItemEvent)
            Dim ActionSuccess As Boolean = pVal.ActionSuccess
            Dim form As Form = B1Connections.theAppl.Forms.Item(pVal.FormUID)
            Dim item As Item = form.Items.Item("6")
            Dim button As Button = CType(item.Specific, Button)
            Dim oMatrix As Matrix
            Dim oCombo As ComboBox
            'ADD YOUR ACTION CODE HERE ...

            form.Freeze(True)
            oMatrix = form.Items.Item("3").Specific
            oMatrix.Clear()

            form.Items.Item("19").Specific.value = ""
            form.Items.Item("1_U_E").Specific.value = ""
            form.Items.Item("2_U_E").Specific.value = ""
            form.Items.Item("17").Specific.value = ""

            oCombo = form.Items.Item("20").Specific
            oCombo.Select("", BoSearchKey.psk_ByValue)

            oCombo = form.Items.Item("1000001").Specific
            oCombo.Select("", BoSearchKey.psk_ByValue)

            form.Items.Item("13").Visible = False
            form.Items.Item("14").Visible = False

            form.Freeze(False)
        End Sub

    End Class
End Namespace
