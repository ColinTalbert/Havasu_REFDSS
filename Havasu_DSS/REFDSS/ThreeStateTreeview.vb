Imports System.ComponentModel

''' <summary>
''' Treeview with 3-state Checkboxes.
''' Get the Treenodes 3-state Checkstate by calling ThreeStateTreeview.GetCheckstate(Treenode)
''' </summary>
Public Class ThreeStateTreeview : Inherits TreeView

   Private _indeterminateds As New List(Of TreeNode)
   Private _graphics As Graphics
   Private _imgIndeterminate As Image
   Private _skipCheckEvents As Boolean = False

   Private Enum CState As Integer
      ' since uninitialized Nodes have StateImageIndex -1, I associate that with 'UnChecked'
      UnChecked = -1
      Checked = 0
      Indeterminate = 1
   End Enum

   Public Sub New()
      InitializeComponent()
      MyBase.DrawMode = TreeViewDrawMode.OwnerDrawAll
      MyBase.CheckBoxes = True
      _imgIndeterminate = ImageList1.Images(2)
      MyBase.StateImageList = ImageList1
   End Sub

   Protected Overrides Sub Dispose(ByVal disposing As Boolean)
      If disposing AndAlso _graphics IsNot Nothing Then
         _graphics.Dispose()
         _graphics = Nothing
         If components IsNot Nothing Then components.Dispose()
      End If
      MyBase.Dispose(disposing)
   End Sub

   Protected Overrides Sub OnHandleCreated(ByVal e As EventArgs)
      MyBase.OnHandleCreated(e)
      _graphics = Me.CreateGraphics
   End Sub

   Protected Overrides Sub OnSizeChanged(ByVal e As EventArgs)
      MyBase.OnSizeChanged(e)
      If _graphics IsNot Nothing Then
         _graphics.Dispose()
         _graphics = Me.CreateGraphics
      End If
   End Sub

   Protected Overrides Sub OnBeforeCheck(ByVal e As System.Windows.Forms.TreeViewCancelEventArgs)
      If _skipCheckEvents Then Return
      'If (e.Node.StateImageIndex = 0) = e.Node.Checked Then Return 'suppress redundant event
      MyBase.OnBeforeCheck(e)
   End Sub

   Protected Overrides Sub OnAfterCheck(ByVal e As TreeViewEventArgs)
      ' Logic: All children of an (un)checked Node inherit its Checkstate
      ' Parents recompute their state: if all children of a parent have same state, that one will be taken over as parents state
      ' otherwise take Indeterminate
      ' changing any Treenodes .Checked-Property will raise another Before- and After-Check. Skip'em
      If _skipCheckEvents Then Return
      _skipCheckEvents = True
      Try
         Dim nd As TreeNode = e.Node
         Dim state As CState
         ' toggle
         If nd.StateImageIndex = CState.Checked Then ' was checked...
            state = CState.UnChecked '...now unchecked
         Else
            state = CState.Checked ' (note: Indeterminate toggles to Checked)
         End If
         If (state = 0) <> e.Node.Checked Then Return 'suppress redundant event
         InheritCheckstate(nd, state) ' inherit Checkstate to children
         ' Parents recompute their state
         nd = nd.Parent
         Do Until nd Is Nothing
            ' At .Indeterminate skip the children-query - every parent becomes .Indeterminate
            If state <> CState.Indeterminate Then
               For Each ndChild As TreeNode In nd.Nodes
                  If ndChild.StateImageIndex <> state Then
                     state = CState.Indeterminate
                     Exit For
                  End If
               Next
            End If
            AssignState(nd, state)
            nd = nd.Parent
         Loop
         MyBase.OnAfterCheck(e)
      Finally
         _skipCheckEvents = False
      End Try
   End Sub

   Private Sub AssignState(ByVal nd As TreeNode, ByVal state As CState)
      Dim ck As Boolean = state = CState.Checked
      Dim stateInvalid As Boolean = nd.StateImageIndex <> state
      If stateInvalid Then nd.StateImageIndex = state
      If nd.Checked <> ck Then
         nd.Checked = ck ' changing .Checked-Property raises Invalidating internally
      ElseIf stateInvalid Then
         ' in general: the less and small the invalidated area, the less flickering
         ' so avoid calling Invalidate() if possible - just call, if really needed.
         Me.Invalidate(GetCheckRect(nd))
      End If
   End Sub

   Private Sub InheritCheckstate(ByVal nd As TreeNode, ByVal state As CState)
      AssignState(nd, state)
      For Each ndChild As TreeNode In nd.Nodes
         InheritCheckstate(ndChild, state)
      Next
   End Sub

   Public Function GetState(ByVal nd As TreeNode) As System.Windows.Forms.CheckState
      ' compute the System.Windows.Forms.CheckState from a StateImageIndex is not that complicated
      Return DirectCast(nd.StateImageIndex + 1, CheckState)
   End Function

   Protected Overrides Sub OnDrawNode(ByVal e As DrawTreeNodeEventArgs)
      ' here nothing is drawn. Only collect Indeterminated Nodes, to draw them later (in WndProc())
      ' because drawing Treenodes properly (Text, Icon(s) Focus, Selection...) is very complicated
      If e.Node.StateImageIndex = CState.Indeterminate Then _indeterminateds.Add(e.Node)
      e.DrawDefault = True
      MyBase.OnDrawNode(e)
   End Sub

   Protected Overrides Sub WndProc(ByRef m As Message)
      Const WM_Paint As Integer = 15
      MyBase.WndProc(m)
      If m.Msg = WM_Paint Then
         ' at that point the built-in drawing is completed - and I quickly paint over the indeterminated Checkboxes
         For Each nd As TreeNode In _indeterminateds
            _graphics.DrawImage(_imgIndeterminate, GetCheckRect(nd).Location)
         Next
         _indeterminateds.Clear()
      End If
   End Sub

   Private Function GetCheckRect(ByVal nd As TreeNode) As Rectangle
      With nd.Bounds
         If Me.ImageList Is Nothing Then
            Return New Rectangle(.X - 16, .Y, 16, 16)
         Else
            Return New Rectangle(.X - 35, .Y, 16, 16)
         End If
      End With
   End Function

   ' since ThreeStateTreeview comes along with its own StateImageList, prevent assigning the StateImageList- Property from outside. Shadow and re-attribute original property
   <EditorBrowsable(EditorBrowsableState.Never)> _
   <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
   Public Shadows Property StateImageList() As ImageList
      Get
         Return MyBase.StateImageList
      End Get
      Set(ByVal value As ImageList)
      End Set
   End Property

#Region "Designergenerated"

   Friend WithEvents ImageList1 As ImageList
   Private components As IContainer
   Private Sub InitializeComponent()
      Me.components = New Container
      Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(ThreeStateTreeview))
      Me.ImageList1 = New ImageList(Me.components)
      Me.SuspendLayout()
      '
      'ImageList1
      '
      Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), ImageListStreamer)
      Me.ImageList1.TransparentColor = Color.Transparent
      Me.ImageList1.Images.SetKeyName(0, "UnChecked.ico")
      Me.ImageList1.Images.SetKeyName(1, "Checked.ico")
      Me.ImageList1.Images.SetKeyName(2, "Indeterminated.ico")
      Me.ResumeLayout(False)
   End Sub
#End Region 'Designergenerated

End Class
