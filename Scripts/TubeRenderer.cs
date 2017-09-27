/*
	Created by Carl Emil Carlsen.
	Copyright 2016 Sixth Sensor.
	All rights reserved.
	http://sixthsensor.dk

	This is a Unity Asset Store product.
		http://u3d.as/2Tc
		https://unity3d.com/legal/as_terms

	Version 1.6

    NovaVR added bug fix in line 407

*/


using UnityEngine;
using System.Collections;

namespace TubeRender {

[ AddComponentMenu( "Effects/TubeRenderer" ) ]
[ RequireComponent( typeof( MeshFilter ) ) ]
[ RequireComponent( typeof( MeshRenderer ) ) ]
[ ExecuteInEditMode ]


public class TubeRenderer : MonoBehaviour
{
	public enum NormalMode { Smooth, Hard, HardEdges }
	public enum CapMode { None, Begin, End, Both }

	[SerializeField] Vector3[] _points = new Vector3[0];
	[SerializeField] float[] _radiuses = new float[0];
	[SerializeField] float _radius = 0.1f;
	[SerializeField] Color32[] _pointColors = new Color32[0];
	[SerializeField] int _edgeCount = 12; // Minimum is three
	[SerializeField] bool _calculateTangents;
	[SerializeField] bool _invertMesh;
	[SerializeField] NormalMode _normalMode = NormalMode.Smooth;
	[SerializeField] CapMode _caps = CapMode.Both;
	[SerializeField] bool _postprocessContinously = true;
	[SerializeField] Rect _uvRect = new Rect(0,0,1,1);
	[SerializeField] Rect _uvRectCap = new Rect(0,0,1,1);
	[SerializeField] bool _uvRectCapEndMirrored;
	[SerializeField] float _forwardAngleOffset;
	[SerializeField] Mesh _mesh;
	[SerializeField] bool _meshGizmos;
	[SerializeField] float _meshGizmoLength = 0.1f;
	[SerializeField] bool _pointsFoldout, _radiusesFoldout, _colorsFoldout, _uvFoldout; // For the inspector

	Vector3[] vertices;
	Vector3[] normals;
	int[] triangles;
	Vector2[] uvs;
	Vector4[] tangents;
	Color32[] colors32;
	Vector3[] circlePointLookup;
	Vector3[] circleNormalLookup;
	Vector3[] circleTangentLookup;
	Quaternion[] rotations;
	Vector3[] directions;
	float[] steepnessAngles;
	Vector3 pastUp;

	MeshFilter filter;

	bool _dirtyCircle = true;
	bool _dirtyRotations = true;
	bool _dirtySteepnessAngles = true;
	bool _dirtyVertexCount = true;
	bool _redrawFlag = true;
	bool _dirtyTriangles = true;
	bool _dirtyUVs = true;
	bool _dirtyColors = true;

	const float tau = Mathf.PI * 2;
	const int meshVertexCountLimit = 65000;
	const string messageHeader = "<b>[TubeRenderer]</b> ";


	////////////
	// PUBLIC //
	////////////

	/// <summary>
	/// Center points for the tube. Forward rotations will be calculated from the point at index 0 and upwards. The array is NOT copied; the tube will keep the reference to the array.
	/// </summary>
	public Vector3[] points {
		get { return _points; }
		set {
			if( value == null ) return;
			if( value.Length == 1 ){
				Debug.LogWarning( messageHeader + "Points change for " + name + " was ignored. Array must have at least two points.\n" );
				return;
			}
			if( value.Length != _points.Length ){
				// Check against Unity mesh limit.
				int targetVertexCount = ComputeVertexCountForProperties( value.Length, _edgeCount, _normalMode, _caps );
				if( targetVertexCount > meshVertexCountLimit ){
					Debug.LogWarning( messageHeader + "Points change for " + name + " was ignored. You are exceeding Unity's 65000 vertex limit.\n" );
					return;
				}
				_dirtyVertexCount = true;
				_dirtyTriangles = true;
				_dirtyUVs = true;
				_dirtyColors = true;
			}
			_dirtyRotations = true;
			_dirtySteepnessAngles = true;
			_redrawFlag = true;
			_points = value;
		}
	}
	
	/// <summary>
	/// Radius values for the tube. Each value corresponds to the point at the same array index. Array length must fit the number of points. If 'radius' has been set then 'radiuses' will be ignored. The array is NOT copied; the tube will keep the reference to the array.
	/// </summary>
	public float[] radiuses {
		get { return _radiuses; }
		set {
			if( value == null ) return;
			if( value == null ){
				_radiuses = new float[0];
			} else {
				_radiuses = value;
				_dirtySteepnessAngles = true;
			}
			_redrawFlag = true;
		}
	}
	
	/// <summary>
	/// Radius for the entire tube. If 'radiuses' has been set then 'radius' will be ignored. Default is 0.1.
	/// </summary>
	public float radius {
		get { return _radius; }
		set {
			if( _radiuses.Length != 0 ){
				_radiuses = new float[0];
				steepnessAngles = null;
			}
			if( value == _radius ) return;
			_redrawFlag = true;
			_radius = value;
		}
	}
	
	/// <summary>
	/// Vertex colors for the tube. Each value corresponds to the point at the same array index. Array length must fit the number of points. The array is NOT copied; the tube will keep the reference to the array.
	/// </summary>
	public Color32[] colors {
		get { return _pointColors; }
		set {
			if( value == null ) return;
			if( value != null && value.Length == 0 ){
				_pointColors = null;
			} else {
				_pointColors = value;
			}
			_dirtyColors = true;
		}
	}
	
	/// <summary>
	/// Edge resolution. Minimum is 3. Default is 12.
	/// </summary>
	public int edgeCount {
		get { return _edgeCount; }
		set {
			if( value == _edgeCount ) return;
			if( value < 3 ){
				Debug.LogWarning( messageHeader + "Edge count change for " + name + " was ignored. A tube must have at least three edges." );
				return;
			}
			// Check against Unity mesh limit.
			int targetVertexCount = ComputeVertexCountForProperties( _points.Length, value, _normalMode, _caps );
			if( targetVertexCount > meshVertexCountLimit ){
				Debug.LogWarning( messageHeader + "Edge count change for " + name + " was ignored. You are exceeding Unity's 65000 vertex limit.\n" );
				return;
			}
			_dirtyCircle = true;
			_dirtyVertexCount = true;
			_redrawFlag = true;
			_dirtyTriangles = true;
			_dirtyUVs = true;
			_dirtyColors = true;
			_edgeCount = value;
		}
	}
	
	/// <summary>
	/// Calculation of tangents. Default is false (to boost performance).
	/// </summary>
	public bool calculateTangents {
		get { return _calculateTangents; }
		set {
			if( value == _calculateTangents ) return;
			if( !value ){
				tangents = null;
				_mesh.tangents = null;
			}
			_dirtyVertexCount = true; // We need UpdateVertexCount call to ensure length of tangent array.
			_redrawFlag = true;
			_calculateTangents = value;
		}
	}
	
	/// <summary>
	/// Mesh inversion (render the tube inside out). In most cases you should do 'Cull Front' in your shader instead. Default is false.
	/// </summary>
	public bool invertMesh {
		get { return _invertMesh; }
		set {
			if( value == _invertMesh ) return;
			_dirtyTriangles = true;
			_redrawFlag = true;
			_invertMesh = value;
		}
	}

	/// <summary>
	/// How normals are rendered. Default is NormalMode.Smooth.
	/// </summary>
	public NormalMode normalMode {
		get { return _normalMode; }
		set {
			if( value == _normalMode ) return;
			// Check against Unity mesh limit.
			int targetVertexCount = ComputeVertexCountForProperties( _points.Length, _edgeCount, value, _caps );
			if( targetVertexCount > meshVertexCountLimit ){
				Debug.LogWarning( messageHeader + "Normal mode change for " + name + " was ignored. You are exceeding Unity's 65000 vertex limit.\n" );
				return;
			}
			_dirtyCircle = true;
			_dirtyVertexCount = true;
			_redrawFlag = true;
			_dirtyTriangles = true;
			_dirtyUVs = true;
			_dirtyColors = true;
			_normalMode = value;
		}
	}

	/// <summary>
	/// Closed end points. Default is true.
	/// </summary>
	public CapMode caps {
		get { return _caps; }
		set {
			if( value == _caps ) return;
			// Check against Unity mesh limit.
			int targetVertexCount = ComputeVertexCountForProperties( _points.Length, _edgeCount, _normalMode, value );
			if( targetVertexCount > meshVertexCountLimit ){
				Debug.LogWarning( messageHeader + "Caps mode change for " + name + " was ignored. You are exceeding Unity's 65000 vertex limit.\n" );
				return;
			}
			_dirtyVertexCount = true;
			_redrawFlag = true;
			_dirtyTriangles = true;
			_dirtyUVs = true;
			_dirtyColors = true;
			_caps = value;
		}
	}
	
	/// <summary>
	/// Postprocess continously (if AddPostprocess has been called). When true, postprocesses will be called every update. When false, they will only be called when tube properties are changed. Default is true.
	/// </summary>
	public bool postprocessContinously {
		get { return _postprocessContinously; }
		set { _postprocessContinously = value; }
	}
	
	/// <summary>
	/// UV mapping rect for wrapped tube body. Default is Rect(0,0,1,1).
	/// </summary>
	public Rect uvRect {
		get { return _uvRect; }
		set {
			if( value == _uvRect ) return;
			_dirtyUVs = true;
			_uvRect = value;
		}
	}
	
	/// <summary>
	/// UV mapping rect for tube caps (if caps is true). Default is Rect(0,0,1,1).
	/// </summary>
	public Rect uvRectCap {
		get { return _uvRectCap; }
		set {
			if( value == _uvRectCap ) return;
			_dirtyUVs = true;
			_uvRectCap = value;
		}
	}
	
	/// <summary>
	/// Mirrored uv mapping for cap at end point (points[points.Length-1]). Default is false.
	/// </summary>
	public bool uvRectCapEndMirrored {
		get { return _uvRectCapEndMirrored; }
		set {
			if( value == _uvRectCapEndMirrored ) return;
			_dirtyUVs = true;
			_uvRectCapEndMirrored = value;
		}
	}
	
	/// <summary>
	/// Rotation offset around the tubes forward direction.
	/// </summary>
	public float forwardAngleOffset {
		get { return _forwardAngleOffset; }
		set {
			if( value == _forwardAngleOffset ) return;
			_dirtyRotations = true;
			_redrawFlag = true;
			_forwardAngleOffset = value;
		}
	}
	
	/// <summary>
	/// Get the tube mesh. Useful for combining multiple tubes into a static mesh. Do not manipulate directly.
	/// </summary>
	public Mesh mesh { get{ return _mesh; } }
	
	/// <summary>
	/// Draw gizmos for mesh normals and tangents. Default is false.
	/// </summary>
	public bool meshGizmos { get { return _meshGizmos; } set { _meshGizmos = value; }  }

	/// <summary>
	/// Length of gizmos for mesh normals and tangents. Default is 0.1.
	/// </summary>
	public float meshGizmoLength { get { return _meshGizmoLength; } set { _meshGizmoLength = value; } }
	
	
	/// <summary>
	/// Force update to generate the tube mesh immediately.
	/// </summary>
	public void ForceUpdate()
	{
		LateUpdate();
	}
	
	
	/// <summary>
	/// Shortcut to Mesh.MarkDynamic(). Call this if the tube will be updated often so that Unity can optimise memory use.
	/// </summary>
	public void MarkDynamic()
	{
		_mesh.MarkDynamic();
	}
	
	
	/// <summary>
	/// Add a method to receive and manipulate mesh data before it is applied. Useful for creating distortion effects or complex variations.
	/// </summary>
	public void AddPostprocess( Postprocess postprocess )
	{
		Postprocesses += postprocess;
	}
	
	
	/// <summary>
	/// Remove a postprocess method that have previously been assigned using the 'AddPostprocess' method.
	/// </summary>
	public void RemovePostprocess( Postprocess postprocess )
	{
		Postprocesses -= postprocess;
	}
	
	/// <summary>
	/// Method for passing mesh data.
	/// </summary>
	public delegate void Postprocess( Vector3[] vertices, Vector3[] normals, Vector4[] tangents );
	Postprocess Postprocesses;


	/// <summary>
	/// Gets the rotation at point.
	/// </summary>
	public Quaternion GetRotationAtPoint( int index )
	{
		if( index < 0 || index > rotations.Length-1 ) return Quaternion.identity;
		return rotations[index];
	}

	
	
	/////////////
	// PRIVATE //
	/////////////
	
	
	void Awake()
	{
		// Ensure that we have a mesh filter.
		filter = gameObject.GetComponent<MeshFilter>();
		if( !filter ) filter = gameObject.AddComponent<MeshFilter>();
		
		// Ensure that we have a mesh. If tube was duplicated or pasted, then make sure it gets a seperate mesh.
		// from NovaVR: next line returns Null Reference Exception outside of OnGUI, so disabled it
		//if( !_mesh || ( Application.isEditor && ( Event.current.commandName == "Duplicate" || Event.current.commandName == "Paste" ) ) ){
		if( !_mesh ) {
			_mesh = new Mesh();
			_mesh.name = "Tube " + gameObject.GetInstanceID();
			filter.sharedMesh = _mesh;
		}
	

		// Ensure that we have mesh renderer and material.
		MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
		if( meshRenderer == null ) meshRenderer = gameObject.AddComponent<MeshRenderer>();
		if( meshRenderer.sharedMaterial == null ) meshRenderer.sharedMaterial = new Material( Shader.Find( "Standard" ) );

		// If the tube was created in the editor, then provide two points.
		if( !Application.isPlaying && points.Length == 0 ) points = new Vector3[]{ Vector3.zero, Vector3.up };
	}
	
	
	void LateUpdate()
	{
		// Return if no points.
		if( _points.Length == 0 ){
			if( _mesh.vertexCount > 0 ) _mesh.Clear();
			return;
		}
		
		// When postprocessing we need to recalculate mesh data every update.
		if( Postprocesses != null && _postprocessContinously ) _redrawFlag = true;
		
		// Update only what needs updating.
		if( _dirtyVertexCount ) UpdateVertexCount();				// Before Everything!
		if( _dirtyRotations ) UpdateRotations();					// Before UpdateSteepnessAngles!
		if( _dirtySteepnessAngles ) UpdateSteepnessAngles();		// Before ReDraw!
		if( _dirtyCircle ) UpdateCircleLookup();					// Before ReDraw!
		if( _redrawFlag ) ReDraw();									// Update vertices, normals and tangents.
		if( _dirtyTriangles ) UpdateTriangles();					// After ReDraw!
		if( _dirtyUVs ) UpdateUVs();								// After ReDraw!
		if( _dirtyColors ) UpdateColors(); 							// After ReDraw!
	}
	
	
	void OnDrawGizmos()
	{
		if( meshGizmos ){
			Gizmos.matrix = transform.localToWorldMatrix;
			for( int v=0; v<vertices.Length; v++ )
			{
				// normals //
				Gizmos.color = new Color( 0, 0, 1, 0.5f );
				Gizmos.DrawLine( vertices[v], vertices[v] + normals[v] * meshGizmoLength );
				
				// tangents //
				if( calculateTangents && tangents.Length != 0 ){
					if( tangents[v].w == -1 ) Gizmos.color = new Color( 1, 0, 0, 0.5f );
					else if( tangents[v].w == 1 ) Gizmos.color = new Color( 1, 1, 0, 0.5f );
					else Gizmos.color = Color.white;
					Gizmos.DrawLine( vertices[v], vertices[v] + new Vector3( tangents[v].x, tangents[v].y, tangents[v].z ) * meshGizmoLength );
				}
			}
		}
	}
	

	// Redraw updates and uploads data that is dependent on point positions and radiuses, that is: vertices, normals and tangets. 
	void ReDraw()
	{		
		int v = 0;
		Matrix4x4 matrix = new Matrix4x4();

		// calculate vertices and update bounds //
		Vector3 minBounds = new Vector3( 10000, 10000, 10000 );
		Vector3 maxBounds = new Vector3( -10000, -10000, -10000 );
		bool usingRadiuses = _radiuses.Length != 0;
		for( int p=0; p<_points.Length; p++ )
		{
			if( usingRadiuses ){
				int rad = p % radiuses.Length;
				// create transform matrix //
				matrix.SetTRS( _points[ p ], rotations[ p ], Vector3.one * _radiuses[ rad ] );
				// check min and max bounds //
				if( _points[ p ].x - _radiuses[ rad ] < minBounds.x ) minBounds.x = _points[ p ].x - _radiuses[ rad ];
				if( _points[ p ].y - _radiuses[ rad ] < minBounds.y ) minBounds.y = _points[ p ].y - _radiuses[ rad ];
				if( _points[ p ].z - _radiuses[ rad ] < minBounds.z ) minBounds.z = _points[ p ].z - _radiuses[ rad ];
				if( _points[ p ].x + _radiuses[ rad ] > maxBounds.x ) maxBounds.x = _points[ p ].x + _radiuses[ rad ];
				if( _points[ p ].y + _radiuses[ rad ] > maxBounds.y ) maxBounds.y = _points[ p ].y + _radiuses[ rad ];
				if( _points[ p ].z + _radiuses[ rad ] > maxBounds.z ) maxBounds.z = _points[ p ].z + _radiuses[ rad ];
			} else {
				// create transform matrix //
				matrix.SetTRS( _points[ p ], rotations[ p ], Vector3.one * _radius );
				// check min and max bounds //
				if( _points[ p ].x - _radius < minBounds.x ) minBounds.x = _points[ p ].x - _radius;
				if( _points[ p ].y - _radius < minBounds.y ) minBounds.y = _points[ p ].y - _radius;
				if( _points[ p ].z - _radius < minBounds.z ) minBounds.z = _points[ p ].z - _radius;
				if( _points[ p ].x + _radius > maxBounds.x ) maxBounds.x = _points[ p ].x + _radius;
				if( _points[ p ].y + _radius > maxBounds.y ) maxBounds.y = _points[ p ].y + _radius;
				if( _points[ p ].z + _radius > maxBounds.z ) maxBounds.z = _points[ p ].z + _radius;
			}
			
			// calculate vertices //
			for( int e=0; e<_edgeCount; e++ ) vertices[v++] = matrix.MultiplyPoint3x4( circlePointLookup[e] );
			vertices[v] = vertices[v-edgeCount]; // uv wrapping //
			v++;
		}

		// add caps //
		switch( _normalMode ){
			case NormalMode.Smooth: break;
			case NormalMode.Hard: v = (_points.Length-1) * _edgeCount * 4; break;
			case NormalMode.HardEdges: v = _points.Length * _edgeCount * 2; break;
		}
		int invertSign = _invertMesh ? -1 : 1;
		if( _caps == CapMode.Both || _caps == CapMode.Begin ){
			Vector3 normal = rotations[0] * Vector3.back * invertSign;
			Vector4 tangent = rotations[0] * Vector3.right;
			tangent.w = -1;
			for( int e=0; e<_edgeCount+1; e++ ){
				vertices[v] = vertices[e];
				normals[v] = normal;
				if( calculateTangents ) tangents[v] = tangent;
				v++;
			}
			vertices[v] = _points[0]; // center vertex
			normals[v] = normal;
			if( calculateTangents ) tangents[v] = tangent;
		}
		if( _caps == CapMode.Both || _caps == CapMode.End ){
			Vector3 normal = rotations[_points.Length-1] * Vector3.forward * invertSign;
			Vector4 tangent = rotations[_points.Length-1] * Vector3.left;
			tangent.w = -1;
			int vBegin = (_points.Length-1)*(_edgeCount+1);
			if( _caps == CapMode.Both ) v++;
			for( int e=0; e<_edgeCount+1; e++ ){
				vertices[v] = vertices[ vBegin+e ];
				normals[v] = normal;
				if( calculateTangents ) tangents[v] = tangent;
				v++;
			}
			vertices[v] = _points[_points.Length-1]; // center vertex
			normals[v] = normal;
			if( calculateTangents ) tangents[v] = tangent;
		}

		// draw tube in requested normal mode //
		switch( _normalMode ){
			case NormalMode.Smooth: ReDrawSmoothNormals(); break;
			case NormalMode.Hard: ReDrawHardNormals(); break;
			case NormalMode.HardEdges: ReDrawHardNormalEdges(); break;
		}
		
		// post process //
		if( Postprocesses != null ) Postprocesses( vertices, normals, tangents );
		
		// update mesh (note that uvs and colors are set in their update methods) //
		_mesh.vertices = vertices;
		_mesh.normals = normals;

		if( calculateTangents ) _mesh.tangents = tangents;

		// update bounds //
		Vector3 boundsSize = new Vector3( maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, maxBounds.z - minBounds.z );
		Vector3 boundsCenter = new Vector3( minBounds.x + boundsSize.x * 0.5f, minBounds.y + boundsSize.y * 0.5f, minBounds.z + boundsSize.z * 0.5f );
		_mesh.bounds = new Bounds( boundsCenter, boundsSize );

		_redrawFlag = false;
	}


	void ReDrawSmoothNormals()
	{
		int invertSign = _invertMesh ? -1 : 1;
		int v = 0;
		bool usingRadiuses = radiuses.Length != 0;
		for( int p=0; p<_points.Length; p++ ) {
			for( int e=0; e<_edgeCount; e++ ){
				if( usingRadiuses ) normals[v] = rotations[p] * Quaternion.AngleAxis( steepnessAngles[p], circleTangentLookup[e] ) * circleNormalLookup[e] * invertSign;
				else normals[v] = rotations[p] * circleNormalLookup[e] * invertSign;
				if( calculateTangents ){
					tangents[v] = Vector3.Cross( rotations[p] * circleTangentLookup[e], normals[v] );
					tangents[v].w = -1;
				}
				v++;
			}
			// uv wrapping
			normals[v] = normals[v-edgeCount];
			if( _calculateTangents ) tangents[v] = tangents[v-edgeCount];
			v++;
		}
	}


	void ReDrawHardNormals()
	{
		// yes, it may seem ugly, but the method below is faster than calculating overlapping vertices //
		int v;

		// store the first two segments temporarily to avoid overwriting what we are about to read //
		Vector3[] verticesTemp = new Vector3[(_edgeCount+1)*2];
		for( int p=0; p<2; p++ ){
			for( int e=0; e<_edgeCount+1; e++ ){
				v = p*(_edgeCount+1)+e;
				verticesTemp[v] = vertices[v];
			}
		}

		// go backwards and copy from already calculated vertices //
		int[] quad = new int[]{ 0, _edgeCount+1, _edgeCount+2, 1 };
		for( int p=_points.Length-2; p>0; p-- ){ // don't copy vertices from first circle
			for( int e=_edgeCount-1; e>=0; e-- ){
				v = p * _edgeCount * 4 + e * 4;
				int lv = p * (_edgeCount+1) + e;
				Vector3 normal = Vector3.Cross( vertices[lv+quad[3]] - vertices[lv+quad[0]], vertices[lv+quad[1]] - vertices[lv+quad[0]] ).normalized;
				for( int q=0; q<4; q++ ){
					vertices[v] = vertices[lv+quad[q]];
					normals[v] = normal;
					v++;
				}
			}
		}

		// copy from the temporary first two segments //
		for( int e=_edgeCount-1; e>=0; e-- ){
			v = e * 4;
			Vector3 normal = Vector3.Cross( verticesTemp[e+quad[3]] - verticesTemp[e], verticesTemp[e+quad[2]] - verticesTemp[e] ).normalized;
			for( int q=0; q<4; q++ ){
				normals[v] = normal;
				vertices[v] = verticesTemp[e+quad[q]];
				v++;
			}
		}

		// calculate tangents //
		if( _calculateTangents ){
			if( _radiuses.Length == 0 ){
				for( int p=0; p<_points.Length-1; p++ ){
					Vector4 tangent = directions[p].normalized;
					tangent.w = -1;
					v = p * _edgeCount * 4;
					for( int e=0; e<_edgeCount; e++ ) for( int q=0; q<4; q++ ) tangents[v++] = tangent;
				}
			} else {
				for( int p=0; p<_points.Length-1; p++ ){
					for( int e=0; e<_edgeCount; e++ ){
						v = p * _edgeCount * 4 + e * 4;
						if( e == 0 ){
							tangents[v] = ( vertices[v+1] - vertices[v] ).normalized;
							tangents[v].w = -1;
						} else {
							tangents[v] = tangents[v-1];
						}
						if( e == _edgeCount-1 ){
							tangents[v+2] = tangents[v-(_edgeCount-1)*4];
						} else {
							tangents[v+2] = ( vertices[v+2] - vertices[v+3] ).normalized;
							tangents[v+2].w = -1;
						}
						tangents[v+1] = tangents[v];
						tangents[v+3] = tangents[v+2];
					}
				}
			}
		}
	}


	void ReDrawHardNormalEdges()
	{
		int v;

		bool usingRadiuses = radiuses.Length != 0;

		// store the first two segments temporarily to avoid overwriting what we are about to read //
		Vector3[] verticesTemp = new Vector3[(_edgeCount+1)*2];
		for( int p=0; p<2; p++ ){
			for( int e=0; e<_edgeCount+1; e++ ){
				v = p * (_edgeCount+1) + e;
				verticesTemp[v] = vertices[v];
			}
		}

		// go backwards and copy from already calculated vertices //
		int invertSign = _invertMesh ? -1 : 1;
		for( int p=_points.Length-1; p>1; p-- ){ // don't copy vertices from first segment
			for( int e=_edgeCount-1; e>=0; e-- ){
				v = p * _edgeCount * 2 + e * 2;
				int lv = p * (_edgeCount+1) + e;
				if( usingRadiuses ) normals[v] = rotations[p] * Quaternion.AngleAxis( steepnessAngles[p], circleTangentLookup[e] ) * circleNormalLookup[e] * invertSign;
				else normals[v] = rotations[p] * circleNormalLookup[e] * invertSign;
				normals[v+1] = normals[v];
				vertices[v] = vertices[lv];
				vertices[v+1] = vertices[lv+1];
			}
		}

		// copy from the temporary first two segments //
		for( int p=1; p>=0; p-- ){
			for( int e=_edgeCount-1; e>=0; e-- ){
				v = p * _edgeCount * 2 + e * 2;
				int lv = p * (_edgeCount+1) + e;
				if( usingRadiuses ) normals[v] = rotations[p] * Quaternion.AngleAxis( steepnessAngles[p], circleTangentLookup[e] ) * circleNormalLookup[e] * invertSign;
				else normals[v] = rotations[p] * circleNormalLookup[e] * invertSign;
				normals[v+1] = normals[v];
				vertices[v] = verticesTemp[lv];
				vertices[v+1] = verticesTemp[lv+1];
			}
		}

		// calculate tangents //
		if( _calculateTangents ){
			if( usingRadiuses ){
				for( int p=0; p<_points.Length; p++ ){
					for( int e=0; e<_edgeCount; e++ ){
						v = p * _edgeCount * 2 + e * 2;
						if( e == 0 ){
							tangents[v] = Vector3.Cross( rotations[p] * circleTangentLookup[e], normals[v] );
							tangents[v].w = -1;
						} else {
							tangents[v] = tangents[v-1];
						}
						if( e == _edgeCount-1 ){
							tangents[v+1] = tangents[v-(_edgeCount-1)*2];
						} else {
							tangents[v+1] = Vector3.Cross( rotations[p] * circleTangentLookup[e], normals[v+1] );
							tangents[v+1].w = -1;
						}
					}
				}
			} else {
				for( int p=0; p<_points.Length; p++ ){
					Vector4 tangent = directions[p].normalized;
					tangent.w = -1;
					v = p * _edgeCount * 2;
					for( int e=0; e<_edgeCount; e++ ){
						tangents[v++] = tangent;
						tangents[v++] = tangent;
					}
				}
			}
		}
	}


	void UpdateVertexCount()
	{
		// Count the number of vertices we need.
		int targetVertexCount = ComputeVertexCountForProperties( _points.Length, _edgeCount, _normalMode, _caps );

		// Update array lengths.
		if( vertices == null || vertices.Length != targetVertexCount ){
			vertices = new Vector3[ targetVertexCount ];
			normals = new Vector3[ targetVertexCount ];
			mesh.Clear();
		}
		if( calculateTangents ) {
			if( tangents == null || tangents.Length != targetVertexCount ) tangents = new Vector4[ targetVertexCount ];
		} else {
			if( tangents == null || tangents.Length != 0 ) tangents = new Vector4[0];
		}

		_dirtyVertexCount = false;
	}
	
	
	void UpdateCircleLookup()
	{
		if( circlePointLookup == null || circlePointLookup.Length != _edgeCount ){
			circlePointLookup = new Vector3[ _edgeCount ];
			circleNormalLookup = new Vector3[ _edgeCount ];
			circleTangentLookup = new Vector3[ _edgeCount ];
		}
		
		float normalizer = 1 / (float) _edgeCount;
		for( int e=0; e<_edgeCount; e++ ) {
			float pointAngle = e * normalizer * tau;
			circlePointLookup[e] = new Vector3( Mathf.Cos( pointAngle ), Mathf.Sin( pointAngle ), 0 );
			if( _normalMode == NormalMode.HardEdges ){
				float normalAngle = pointAngle + normalizer * Mathf.PI;
				circleNormalLookup[e] = new Vector3( Mathf.Cos( normalAngle ), Mathf.Sin( normalAngle ), 0 );
			} else {
				circleNormalLookup[e] = circlePointLookup[e];
			}
			circleTangentLookup[e] = Vector3.Cross( circleNormalLookup[e], Vector3.forward );
		}

		_dirtyCircle = false;
	}
	
	
	void UpdateRotations()
	{
		// update array lengths //
		if( rotations == null || _points.Length != rotations.Length ){
			rotations = new Quaternion[ _points.Length ];
			directions = new Vector3[ _points.Length ];
		}
		
		// calculate directions //
		for( int p=0; p<points.Length-1; p++ ) directions[p].Set( _points[p+1].x - _points[p].x, _points[p+1].y - _points[p].y, _points[p+1].z - _points[p].z );
		
		// fix directions for doubled points //
		for( int p=0; p<points.Length-1; p++ ){
			if( directions[p] == Vector3.zero ){
				if( p+1 < directions.Length-2 && directions[p+1] != Vector3.zero ){
					directions[p].Set( directions[p+1].x, directions[p+1].y, directions[p+1].z );
				} else if( p-1 > 0 && directions[p-1] != Vector3.zero ){
					directions[p].Set( directions[p-1].x, directions[p-1].y, directions[p-1].z );
				}
			}
		}
		
		// doublicate last direction
		directions[ _points.Length-1 ] = directions[ _points.Length-2 ];
		
		// if the past up direction has not been set in last frame then use default
		Vector3 up;
		if( pastUp == Vector3.zero ){
			up = directions[0].x == 0 && directions[0].z == 0 ? Vector3.right : Vector3.up;
		} else {;
			up = pastUp;
		}

		Vector3 forward = Vector3.zero;
		for( int p=0; p<points.Length; p++ )
		{
			// calculate averaged forward direction
			if( p != 0 && p != _points.Length-1 ){
				forward.Set( directions[p].x + directions[p-1].x, directions[p].y + directions[p-1].y, directions[p].z + directions[p-1].z );
			} else {
				// this is the start or end point, check for tube loop
				if( _points[0] == _points[ _points.Length-1 ] ) forward.Set( directions[_points.Length-1].x + directions[0].x, directions[_points.Length-1].y + directions[0].y, directions[_points.Length-1].z + directions[0].z );
				else forward.Set( directions[p].x, directions[p].y, directions[p].z );
			}
			
			// If the forward vector is zero (probably because the last
			// point was at the same position at this point) we don't
			// mind calculating the rotation.
			if( forward == Vector3.zero ){
				rotations[p] = Quaternion.identity;
				continue;
			}

			forward.Normalize();
			
			// To find the optimal up-rotation of the circle plane we do the following:
			// The cross product of last points up and forward gives us a vector that is rotated 90
			// degrees right on the forward axis from the new up vector. Taking the cross 
			// product of forward and right gives us the new up vector.
			// Vector3 up = Vector3.Cross( Vector3.forward, Vector3.right );
			// http://en.wikipedia.org/wiki/Right-hand_rule

			Vector3 right = Vector3.Cross( up, forward );
			up = Vector3.Cross( forward, right );
			
			if( p == 0 ){
				// store the up direction for the first point
				pastUp = up;
				// offset
				if( _forwardAngleOffset != 0 ) up = Quaternion.AngleAxis( _forwardAngleOffset, forward ) * up;
			}
			
			// create a Quaternion rotation using LookRotation
			rotations[p].SetLookRotation( forward, up );
		}

		_dirtyRotations = false;
	}


	void UpdateSteepnessAngles()
	{
		if( steepnessAngles == null || steepnessAngles.Length != _points.Length ) steepnessAngles = new float[ _points.Length ];

		// If we have the same radius then all steepness angles will be zero.
		if( _radiuses.Length == 0 ) return;

		float[] radiusDiffs = new float[ _points.Length-1 ];
		int radiusCount = _radiuses.Length;
		for( int p=0; p<_points.Length-1; p++ ){
			radiusDiffs[p] = _radiuses[ (p+1)%radiusCount ] - _radiuses[ p%radiusCount ];
		}

		for( int p=0; p<_points.Length-1; p++ ){
			float avgRadiusDiff;
			if( p == 0 ) avgRadiusDiff = radiusDiffs[0];
			else avgRadiusDiff = ( radiusDiffs[p] + radiusDiffs[p-1] ) * 0.5f;
			if( avgRadiusDiff == 0 ) steepnessAngles[p] = 0;
			else steepnessAngles[p] = -Mathf.Atan2( avgRadiusDiff, directions[p].magnitude ) * Mathf.Rad2Deg;
		}
		steepnessAngles[_points.Length-1] = -Mathf.Atan2( radiusDiffs[radiusDiffs.Length-1], directions[directions.Length-1].magnitude ) * Mathf.Rad2Deg;

		_dirtySteepnessAngles = false;
	}
	
	
	void UpdateTriangles()
	{
		// Update array length
		int triangleCount = (points.Length-1) * _edgeCount * 3 * 2;
		if( _caps == CapMode.Both || _caps == CapMode.Begin ) triangleCount += _edgeCount * 3;
		if( _caps == CapMode.Both || _caps == CapMode.End ) triangleCount += _edgeCount * 3;
		if( triangles == null || triangles.Length != triangleCount ) triangles = new int[triangleCount];

		// Stitch the tube
		int v=0; int t=0;
		int[] quad;
		switch( _normalMode ){

		case NormalMode.Smooth:
			if(!_invertMesh) quad = new int[]{ 0, 1, _edgeCount+2, 0, _edgeCount+2, _edgeCount+1 };
			else quad = new int[]{ 0, _edgeCount+2, 1, 0, _edgeCount+1, _edgeCount+2 };
			for( int p=0; p<points.Length-1; p++ ){
				for( int e=0; e<_edgeCount; e++ ){
					for( int q = 0; q < quad.Length; q++ ) triangles[ t++ ] = v + quad[ q ];
					v++;
				}
				v++; // skip hidden vertex
			}
			v += _edgeCount+1; // skip last point
			break;

		case NormalMode.Hard:
			if(!_invertMesh) quad = new int[]{ 0, 3, 1, 3, 2, 1 };
			else quad = new int[]{ 0, 1, 3, 3, 1, 2 };
			for( int p=0; p<points.Length-1; p++ ){
				for( int e=0; e<_edgeCount; e++ ){
					for( int q = 0; q < quad.Length; q++ ) triangles[ t++ ] = v + quad[ q ];
					v += 4;
				}
			}
			break;

		case NormalMode.HardEdges:
			if(!_invertMesh) quad = new int[]{ 0, 1, _edgeCount*2, _edgeCount*2, 1, _edgeCount*2+1 };
			else quad = new int[]{ 0, _edgeCount*2, 1, 1, _edgeCount*2, _edgeCount*2+1 };
			for( int p=0; p<points.Length-1; p++ ){
				for( int e=0; e<_edgeCount; e++ ){
					for( int q = 0; q < quad.Length; q++ ) triangles[ t++ ] = v + quad[ q ];
					v += 2;
				}
			}
			v += _edgeCount*2;
			break;

		}

		// stitch the begin cap //
		if( _caps  == CapMode.Both || _caps  == CapMode.Begin ){
			int vCenter = v + _edgeCount+1;
			if(!_invertMesh){ // ugly but fast
				for( int e=0; e<_edgeCount; e++ ){
					triangles[ t++ ] = v;
					triangles[ t++ ] = vCenter;
					triangles[ t++ ] = v+1;
					v++;
				}
			} else {
				for( int e=0; e<_edgeCount; e++ ){
					triangles[ t++ ] = v;
					triangles[ t++ ] = v+1;
					triangles[ t++ ] = vCenter;
					v++;
				}
			}
		}

		// stitch the end cap //
		if( _caps  == CapMode.Both || _caps  == CapMode.End ){
			if( _caps  == CapMode.Both ){
				v++; // skip hidden vertex
				v++; // skip center vertex
			}
			int vCenter = v + _edgeCount+1;
			if(!_invertMesh){ // ugly but fast
				for( int e=0; e<_edgeCount; e++ ){
					triangles[ t++ ] = v;
					triangles[ t++ ] = v+1;
					triangles[ t++ ] = vCenter;
					v++;
				}
			} else {
				for( int e=0; e<_edgeCount; e++ ){
					triangles[ t++ ] = v;
					triangles[ t++ ] = vCenter;
					triangles[ t++ ] = v+1;
					v++;
				}
			}
		}

		// Upload
		_mesh.triangles = triangles;

		_dirtyTriangles = false;
	}


	void UpdateUVs()
	{
		float u, v;
		if( uvs == null || uvs.Length != vertices.Length ) uvs = new Vector2[ vertices.Length ];
		int uv = 0;
		float uNormalizer = 1 / ( _points.Length -1f );
		float vNormalizer = 1 / (float) _edgeCount;
		
		switch( _normalMode ){
			
		case NormalMode.Smooth:
			for( int p=0; p<points.Length; p++ ){
				u =  _uvRect.xMin + _uvRect.width * (p*uNormalizer);
				for( int e=0; e<_edgeCount+1; e++ ){
					v = _uvRect.yMin + _uvRect.height * (e*vNormalizer);
					uvs[ uv++ ] = new Vector2( u, v );
				}
			}
			break;
			
		case NormalMode.Hard:
			for( int p=0; p<points.Length-1; p++ ){
				u =  _uvRect.xMin + _uvRect.width * (p*uNormalizer);
				float nextU = _uvRect.xMin + _uvRect.width * ((p+1)*uNormalizer);
				for( int e=0; e<_edgeCount; e++ ){
					v = _uvRect.yMin + _uvRect.height * (e*vNormalizer);
					float nextV = _uvRect.yMin + _uvRect.height * ((e+1)*vNormalizer);
					uvs[ uv++ ] = new Vector2( u, v );
					uvs[ uv++ ] = new Vector2( nextU, v );
					uvs[ uv++ ] = new Vector2( nextU, nextV );
					uvs[ uv++ ] = new Vector2( u, nextV );
				}
			}
			break;
		case NormalMode.HardEdges:
			for( int p=0; p<points.Length; p++ ){
				u =  _uvRect.xMin + _uvRect.width * (p*uNormalizer);
				for( int e=0; e<_edgeCount; e++ ){
					v = _uvRect.yMin + _uvRect.height * (e*vNormalizer);
					float nextV = _uvRect.yMin + _uvRect.height * ((e+1)*vNormalizer);
					uvs[ uv++ ] = new Vector2( u, v );
					uvs[ uv++ ] = new Vector2( u, nextV );
				}
			}
			break;
		}

		if( _caps == CapMode.Both || _caps  == CapMode.Begin ){
			for( int e=0; e<_edgeCount; e++ ){
				u = _uvRectCap.yMin + _uvRectCap.height * ( circlePointLookup[e].y*0.5f+0.5f );
				v = _uvRectCap.xMin + _uvRectCap.width * ( 1-(circlePointLookup[e].x*0.5f+0.5f) );
				uvs[ uv++ ] = new Vector2( u, v );
			}
			uvs[uv] = uvs[uv-_edgeCount]; // uv wrap
			uv++;
			u = _uvRectCap.yMin + _uvRectCap.height * 0.5f;
			v = _uvRectCap.xMin + _uvRectCap.width * 0.5f;
			uvs[ uv++ ] = new Vector2( u, v ); // center
		}
		
		if( _caps == CapMode.Both || _caps  == CapMode.End ){
			for( int e=0; e<_edgeCount; e++ ){
				if( _uvRectCapEndMirrored ) u = _uvRectCap.yMin + _uvRectCap.height * ( circlePointLookup[e].y*0.5f+0.5f );
				else u = _uvRectCap.yMin + _uvRectCap.height * ( 1-(circlePointLookup[e].y*0.5f+0.5f) );
				v = _uvRectCap.xMin + _uvRectCap.width * ( 1-(circlePointLookup[e].x*0.5f+0.5f) );
				uvs[ uv++ ] = new Vector2( u, v );
			}
			uvs[uv] = uvs[uv-_edgeCount]; // uv wrap
			uv++;
			u = _uvRectCap.yMin + _uvRectCap.height * 0.5f;
			v = _uvRectCap.xMin + _uvRectCap.width * 0.5f;
			uvs[ uv++ ] = new Vector2( u, v ); // center
		}
		
		_mesh.uv = uvs;

		_dirtyUVs = false;
	}

	
	void UpdateColors()
	{
		if( _pointColors.Length == 0 ){
			if( mesh.colors32.Length != 0 ) mesh.colors32 = new Color32[0];
			if( colors32 != null ) colors32 = null;
			_dirtyColors = false;
			return;
		}
			
		if( colors32 == null || colors32.Length != vertices.Length ) colors32 = new Color32[ vertices.Length ];

		int v = 0;
		switch( _normalMode ){

		case NormalMode.Smooth:
			for( int p=0; p<_points.Length; p++ ) {
				int c = p % _pointColors.Length;
				for( int s=0; s<_edgeCount+1; s++ ) colors32[ v++ ] = _pointColors[ c ];
			}
			break;

		case NormalMode.Hard:
			for( int p=0; p<_points.Length-1; p++ ) {
				int c0 = p % _pointColors.Length;
				int c1 = (p+1) % _pointColors.Length;
				for( int s=0; s<_edgeCount; s++ ){
					colors32[ v++ ] = _pointColors[ c0 ];
					colors32[ v++ ] = _pointColors[ c1 ];
					colors32[ v++ ] = _pointColors[ c1 ];
					colors32[ v++ ] = _pointColors[ c0 ];
				}
			}
			break;

		case NormalMode.HardEdges:
			for( int p=0; p<_points.Length; p++ ) {
				int c = p % _pointColors.Length;
				for( int s=0; s<_edgeCount; s++ ){
					colors32[ v++ ] = _pointColors[c];
					colors32[ v++ ] = _pointColors[c];
				}
			}
			break;
		}

		if( _caps == CapMode.Both || _caps == CapMode.Begin ){
			for( int s=0; s<_edgeCount+2; s++ ) colors32[ v++ ] = _pointColors[ 0 ]; // start cap
		}
		if( _caps == CapMode.Both || _caps == CapMode.End ){
			for( int s=0; s<_edgeCount+2; s++ ) colors32[ v++ ] = _pointColors[ (_points.Length-1) % colors.Length ]; // end cap
		}

		mesh.colors32 = colors32;

		_dirtyColors = false;
	}


	static int ComputeVertexCountForProperties( int pointCount, int edgeCount, NormalMode normalMode, CapMode capMode )
	{
		int targetVertexCount = 0;

		switch( normalMode ){
		case NormalMode.Smooth: targetVertexCount = pointCount * (edgeCount+1); break;
		case NormalMode.Hard: targetVertexCount = (pointCount-1) * edgeCount * 4; break;
		case NormalMode.HardEdges: targetVertexCount = pointCount * 2 * edgeCount; break;
		}
		if( capMode == CapMode.Both ) targetVertexCount += ( (edgeCount+1) + 1 ) * 2;
		else if( capMode == CapMode.Begin || capMode == CapMode.End ) targetVertexCount += (edgeCount+1) + 1;

		return targetVertexCount;
	}


	// When the user makes a change in the inspector
	void OnValidate()
	{
		_dirtyCircle = true;
		_dirtyRotations = true;
		_dirtySteepnessAngles = true;
		_dirtyVertexCount = true;
		_redrawFlag = true;
		_dirtyTriangles = true;
		_dirtyUVs = true;
		_dirtyColors = true;

		if( !_calculateTangents && _mesh.tangents != null ) _mesh.tangents = null;
	}

}

}