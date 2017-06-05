<AttributeUsage(AttributeTargets.Method, AllowMultiple:=True)>
Public Class ParameterDescriptionAttribute
    Inherits Attribute

    Public Sub New(Name As String, Description As String)
        _Name = Name
        _Description = Description
    End Sub

#Region "IsOptional Property"
    Private _IsOptional As Boolean?

    Public Property IsOptional As Boolean
        Get
            Return Me._IsOptional.Value
        End Get
        Set(ByVal Value As Boolean)
            If Me._IsOptional.HasValue Then
                Throw New InvalidOperationException($"Cannot set {NameOf(Me.IsOptional)} twice.")
            End If
            Me._IsOptional = Value
        End Set
    End Property
#End Region

#Region "DefaultValue Property"
    Private _DefaultValue As String
    Private _DefaultValueSet As Boolean

    Public Property DefaultValue As String
        Get
            Return Me._DefaultValue
        End Get
        Set(ByVal Value As String)
            If Me._DefaultValueSet Then
                Throw New InvalidOperationException($"Cannot set {NameOf(Me.DefaultValue)} twice.")
            End If
            Me._DefaultValue = Value
            Me._DefaultValueSet = True
        End Set
    End Property
#End Region

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
