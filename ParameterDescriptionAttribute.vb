<AttributeUsage(AttributeTargets.Method, AllowMultiple:=True)>
Public Class ParameterDescriptionAttribute
    Inherits Attribute

    Public Sub New(Name As String, Description As String)
        _Name = Name
        _Description = Description
    End Sub

#Region "Name Read-Only Property"
    Private ReadOnly _Name As String

    Public ReadOnly Property Name As String
        Get
            Return Me._Name
        End Get
    End Property
#End Region

#Region "Description Read-Only Property"
    Private ReadOnly _Description As String

    Public ReadOnly Property Description As String
        Get
            Return Me._Description
        End Get
    End Property
#End Region

End Class
