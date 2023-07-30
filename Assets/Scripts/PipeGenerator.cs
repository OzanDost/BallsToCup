using System;
using System.IO;
using Dreamteck;
using Dreamteck.Splines;
using Unity.VectorGraphics;
using UnityEngine;

public class PipeGenerator : MonoBehaviour
{
    public SplineComputer SplineComputer => _splineComputer;
    public Material Material => _material;

    [SerializeField] private Material _material;
    [SerializeField] private SplineComputer _splineComputer;
    [SerializeField] private SplineMesh _splineMesh;
    [SerializeField] private float _scaleFactor = 0.05f;
    [SerializeField] private MeshFilter _filter;

    public Mesh Generate(string svgText)
    {
        _splineComputer.SetPoints(Array.Empty<SplinePoint>());

        var textReader = new StringReader(svgText);
        var sceneInfo = SVGParser.ImportSVG(textReader);
        
        var svgPath = sceneInfo.Scene.Root.Children[0];
        var segments = svgPath.Shapes[0].Contours[0].Segments;

        int index = 0;
        foreach (var segment in segments)
        {
            if (index != segments.Length - 1)
            {
                _splineComputer.SetPoint(index, new SplinePoint(segment.P0 * _scaleFactor, segment.P1 * _scaleFactor));
            }
            else
            {
                _splineComputer.SetPoint(index, new SplinePoint(segment.P0 * _scaleFactor));
            }

            index++;
        }

        _splineComputer.RebuildImmediate();

        _splineMesh.RebuildImmediate();

        return MeshUtility.Copy(_filter.sharedMesh);
    }
}