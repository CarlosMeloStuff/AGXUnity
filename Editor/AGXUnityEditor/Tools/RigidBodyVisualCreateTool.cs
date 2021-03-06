﻿using UnityEngine;
using UnityEditor;
using AGXUnity;
using AGXUnity.Collide;
using AGXUnity.Rendering;
using GUI = AGXUnityEditor.Utils.GUI;

namespace AGXUnityEditor.Tools
{
  public class RigidBodyVisualCreateTool : Tool
  {
    public static bool ValidForNewShapeVisuals( RigidBody rb )
    {
      if ( rb == null )
        return false;

      var shapes = rb.GetComponentsInChildren<Shape>();
      int numSupportedNewShapeVisual = 0;
      foreach ( var shape in shapes )
        numSupportedNewShapeVisual += System.Convert.ToInt32( ShapeVisualCreateTool.CanCreateVisual( shape ) );

      return numSupportedNewShapeVisual > 0;
    }

    public RigidBody RigidBody { get; private set; }

    public RigidBodyVisualCreateTool( RigidBody rb )
    {
      RigidBody = rb;
    }

    public override void OnAdd()
    {
      var shapes = RigidBody.GetComponentsInChildren<Shape>();
      foreach ( var shape in shapes )
        AddChild( new ShapeVisualCreateTool( shape ) );
    }

    public override void OnRemove()
    {
    }

    public void OnInspectorGUI( GUISkin skin )
    {
      if ( RigidBody == null || GetChildren().Length == 0 ) {
        PerformRemoveFromParent();
        return;
      }

      GUILayout.Space( 4 );
      using ( GUI.AlignBlock.Center )
        GUILayout.Label( GUI.MakeLabel( "Create visual tool", 16, true ), skin.label );

      GUILayout.Space( 2 );
      GUI.Separator();
      GUILayout.Space( 4 );

      foreach ( var tool in GetChildren<ShapeVisualCreateTool>() ) {
        if ( ShapeVisual.HasShapeVisual( tool.Shape ) )
          continue;

        using ( GUI.AlignBlock.Center )
          GUILayout.Label( GUI.MakeLabel( tool.Shape.name, 16, true ), skin.label );
        tool.OnInspectorGUI( skin, true );
      }

      var createCancelState = GUI.CreateCancelButtons( true, skin, "Create shape visual for shapes that hasn't already got one." );
      if ( createCancelState == GUI.CreateCancelState.Create ) {
        foreach ( var tool in GetChildren<ShapeVisualCreateTool>() )
          if ( !ShapeVisual.HasShapeVisual( tool.Shape ) )
            tool.CreateShapeVisual();
      }
      if ( createCancelState != GUI.CreateCancelState.Nothing )
        PerformRemoveFromParent();
    }
  }
}
