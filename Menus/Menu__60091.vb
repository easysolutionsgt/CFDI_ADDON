'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.34014
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

    Public Class Menu__60091 'Asistente de confirmación de provisionamiento
        Inherits B1Form

        Public Sub New()
            MyBase.New()
            FormType = "60091"
        End Sub

        <B1Listener(BoEventTypes.et_FORM_LOAD, True)>
        Public Overridable Function OnBeforeFormLoad(ByVal pVal As ItemEvent) As Boolean
            Dim form As Form = B1Connections.theAppl.Forms.Item(pVal.FormUID)
            Dim item1 As Item
            Dim iTop1 As Integer
            Dim iLeft1 As Integer
            Dim iHeight1 As Integer
            'ADD YOUR ACTION CODE HERE ...

            item1 = form.Items.Item("10000330")

            iTop1 = form.Height - 35
            iHeight1 = item1.Height
            iLeft1 = form.Width - 208



            SAPSDKProg.Form.Agregar.Button_con_definicion_manual(pVal, "btnPDF", iTop1, iLeft1 - 139, iHeight1, 70, True, "Visualizar PDF", BoButtonTypes.bt_Caption, 0, 0)

            Return True
        End Function
    End Class
End Namespace