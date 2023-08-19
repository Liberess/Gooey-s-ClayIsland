// Name: Robert MacGillivray
// File: BoundaryNode.cs
// Date: Jul.21.2016
// Purpose: To do anything specific to just nodes in boundaries (like calling OnDrawGizmoSelected on the boundary when you're fiddling with the node)

// Last Updated: Jul.01.2022 by Robert MacGillivray

using UnityEngine;

namespace UmbraEvolution.UmbraBoundaryBuilder
{
    /// <summary>
    /// Handles functionality local to nodes. Takes ownership of a specific section of a boundary.
    /// </summary>
    public class BoundaryNode : MonoBehaviour
    {
        private Boundary _parent;
        public Boundary ParentBoundary
        {
            get
            {
                if (_parent == null)
                {
                    _parent = transform.parent.GetComponent<Boundary>();
                    if (_parent == null)
                    {
                        Debug.LogErrorFormat("The BoundaryNode [{0}] does not have a Boundary parent. This BoundaryNode will not work correctly.", gameObject.name);
                    }
                }
                return _parent;
            }
        }

        private BoxCollider _myBoxCollider;
        private BoxCollider MyBoxCollider
        {
            get
            {
                if (_myBoxCollider == null)
                {
                    _myBoxCollider = GetComponent<BoxCollider>();
                    if (_myBoxCollider == null)
                    {
                        _myBoxCollider = gameObject.AddComponent<BoxCollider>();
                    }
                }
                return _myBoxCollider;
            }

            set { _myBoxCollider = value; }
        }

        private GameObject PhysicalNodeGizmo { get; set; }
        private GameObject PhysicalBoundaryGizmo { get; set; }
        private Vector3 LastKnownPosition { get; set; }
        private Quaternion LastKnownRotation { get; set; }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            //Useful when moving nodes so that you can still see the full boundary gizmo and your effects on it
            if (UnityEditor.Selection.activeObject == gameObject)
                ParentBoundary.OnDrawGizmosSelected();
        }
#endif

        /// <summary>
        /// Returns true if this node has moved or rotated since the last time this function was called
        /// </summary>
        /// <returns></returns>
        public bool HasNodeMovedOrRotated()
        {
            if (LastKnownPosition != transform.position || LastKnownRotation != transform.rotation)
            {
                LastKnownPosition = transform.position;
                LastKnownRotation = transform.rotation;
                return true;
            }

            return false;
        }

        /// <summary>
        /// To assist in drawing gizmos in a reverse-recursive fashion without infinite overhead
        /// </summary>
        public void DrawNodeGizmosManually()
        {
            Gizmos.matrix = Matrix4x4.identity;
            if (ParentBoundary)
            {
                Gizmos.color = ParentBoundary.nodeGizmoColour;
                Gizmos.DrawSphere(transform.position, ParentBoundary.nodeGizmoRadius);
            }
            else
            {
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(transform.position, 1f);
            }
        }

        /// <summary>
        /// Used to update or create a physical gizmo for this node object
        /// </summary>
        public void DrawPhysicalNodeGizmosManually()
        {
            // First, make sure we don't have a rogue gizmo that we should know about
            if (!PhysicalNodeGizmo)
            {
                Transform gizmoTransform = transform.Find(BoundaryBuilder.PHYSICAL_NODE_GIZMO_NAME);
                PhysicalNodeGizmo = gizmoTransform != null ? gizmoTransform.gameObject : null;
            }

            // If we're still sure we don't have a gizmo, make one
            if (!PhysicalNodeGizmo)
            {
                PhysicalNodeGizmo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                PhysicalNodeGizmo.name = BoundaryBuilder.PHYSICAL_NODE_GIZMO_NAME;
                DestroyImmediate(PhysicalNodeGizmo.GetComponent<Collider>());
                PhysicalNodeGizmo.transform.SetParent(transform);
                PhysicalNodeGizmo.transform.position = transform.position;
                PhysicalNodeGizmo.transform.rotation = transform.rotation;

                Renderer tempRenderer = PhysicalNodeGizmo.GetComponent<Renderer>();
                tempRenderer.sharedMaterial = new Material(ParentBoundary.ParentBoundaryBuilder.physicalGizmoMaterial);
            }

            // Set Gizmo properties in real-time
            if (ParentBoundary)
            {
                PhysicalNodeGizmo.GetComponent<Renderer>().sharedMaterial.color = ParentBoundary.nodeGizmoColour;
                PhysicalNodeGizmo.transform.localScale = Vector3.one * 2f * ParentBoundary.nodeGizmoRadius;
            }
            else
            {
                PhysicalNodeGizmo.GetComponent<Renderer>().sharedMaterial.color = Color.black;
                PhysicalNodeGizmo.transform.localScale = Vector3.one * 2f;
            }
        }

        /// <summary>
        /// Used to update or create a physical gizmo for the chunk of boundary associated with this node
        /// </summary>
        public void DrawPhysicalBoundaryNodeGizmosManually(Vector3 gizmoSize, Vector3 localCenter)
        {
            // First, make sure we don't have a rogue gizmo that we should know about
            if (!PhysicalBoundaryGizmo)
            {
                Transform gizmoTransform = transform.Find(BoundaryBuilder.PHYSICAL_BOUNDARY_GIZMO_NAME);
                PhysicalBoundaryGizmo = gizmoTransform != null ? gizmoTransform.gameObject : null;
            }

            // If we're still sure we don't have a gizmo, make one
            if (!PhysicalBoundaryGizmo)
            {
                PhysicalBoundaryGizmo = GameObject.CreatePrimitive(PrimitiveType.Cube);
                PhysicalBoundaryGizmo.name = BoundaryBuilder.PHYSICAL_BOUNDARY_GIZMO_NAME;
                DestroyImmediate(PhysicalBoundaryGizmo.GetComponent<Collider>());
                PhysicalBoundaryGizmo.transform.SetParent(transform);
                PhysicalBoundaryGizmo.transform.position = transform.position;
                PhysicalBoundaryGizmo.transform.rotation = transform.rotation;

                Renderer tempRenderer = PhysicalBoundaryGizmo.GetComponent<Renderer>();
                tempRenderer.sharedMaterial = new Material(ParentBoundary.ParentBoundaryBuilder.physicalGizmoMaterial);
            }

            // Set Gizmo properties in real-time
            if (ParentBoundary)
            {
                PhysicalBoundaryGizmo.GetComponent<Renderer>().sharedMaterial.color = ParentBoundary.boundaryGizmoColour;
            }
            else
            {
                PhysicalBoundaryGizmo.GetComponent<Renderer>().sharedMaterial.color = Color.black;
            }

            transform.localScale = Vector3.one;
            PhysicalBoundaryGizmo.transform.localPosition = localCenter;
            PhysicalBoundaryGizmo.transform.localScale = gizmoSize;
        }

        /// <summary>
        /// Cleans up this node's physical gizmos at runtime
        /// </summary>
        public void DestroyPhysicalGizmos()
        {
            if (PhysicalNodeGizmo)
                Destroy(PhysicalNodeGizmo);

            if (PhysicalBoundaryGizmo)
                Destroy(PhysicalBoundaryGizmo);
        }

        /// <summary>
        /// Cleans up this node's physical gizmos in the editor
        /// </summary>
        public void DestroyPhysicalGizmosImmediate()
        {
            if (PhysicalNodeGizmo)
                DestroyImmediate(PhysicalNodeGizmo);

            if (PhysicalBoundaryGizmo)
                DestroyImmediate(PhysicalBoundaryGizmo);
        }

        public void UpdateBoxCollider(float boundaryThickness, float boundaryHeight, float verticalOffset, BoundaryNode nextNode, bool useBoxColliders)
        {
            if (nextNode == null)
            {
                Debug.LogError("A null node was passed to BoundaryNode.UpdateBoxCollider(...). Returning.");
                return;
            }

            float distanceBetweenNodes = Vector3.Distance(transform.position, new Vector3(nextNode.transform.position.x, transform.position.y, nextNode.transform.position.z)) + boundaryThickness;
            
            // Calculates a local center for this node's box collider
            // This should line the collider up with the lower of this node and the next node
            float colliderYPos = Mathf.Min(transform.position.y, nextNode.transform.position.y) + (boundaryHeight / 2f) + verticalOffset - transform.position.y;

            Vector3 size = new Vector3(boundaryThickness, boundaryHeight, distanceBetweenNodes);
            Vector3 center = new Vector3(0f, colliderYPos, distanceBetweenNodes / 2f);

            MyBoxCollider.size = size;
            MyBoxCollider.center = center;
            MyBoxCollider.enabled = useBoxColliders;
        }

        public void DisableBoxCollider()
        {
            MyBoxCollider.size = Vector3.zero;
            MyBoxCollider.center = Vector3.zero;
            MyBoxCollider.enabled = false;
        }

        /// <summary>
        /// Rotates this node to look at the given node.
        /// </summary>
        /// <param name="nextNode">The node that this node is connected to.</param>
        public void CalculateAndSetRotation(BoundaryNode nextNode)
        {
            if (nextNode == null)
            {
                Debug.LogError("A null node was passed to BoundaryNode.CalculateAndSetRotation(...). Returning.");
                return;
            }

            if (transform.position.x == nextNode.transform.position.x && transform.position.z == nextNode.transform.position.z)
            {
                Debug.LogErrorFormat("Two connected nodes are stacked on top of each other at [{0}] and [{1}]. A proper boundary segment cannot be created if the X and Z coordinates are the same.", transform.position, nextNode.transform.position);
                return;
            }

            // Look at the next node, ignoring height differences
            transform.rotation = Quaternion.LookRotation(new Vector3(nextNode.transform.position.x, transform.position.y, nextNode.transform.position.z) - transform.position, Vector3.up);
        }

        /// <summary>
        /// A method with a lot of hard-coded shtick to place vertices that this node is responsible for while generating the invisible mesh used to bake the navmesh
        /// </summary>
        /// <param name="heightAdjustment">Vertical offset from this node to place the vertices. Used when the next node is lower than this node.</param>
        /// <returns>Returns an array of 8 vertices used to generate mesh. Will match the vertices of the box collider component on this node.</returns>
        public Vector3[] ReturnVertices(float heightAdjustment)
        {
            Vector3[] vertices = new Vector3[8];

            vertices[0] = transform.position + (-transform.forward * ParentBoundary.boundaryThickness / 2f) + (transform.right * ParentBoundary.boundaryThickness / 2f) + (transform.up * (ParentBoundary.verticalOffset + heightAdjustment));
            vertices[1] = transform.position + (-transform.forward * ParentBoundary.boundaryThickness / 2f) + (-transform.right * ParentBoundary.boundaryThickness / 2f) + (transform.up * (ParentBoundary.verticalOffset + heightAdjustment));
            vertices[2] = vertices[0] + (transform.up * ParentBoundary.boundaryHeight);
            vertices[3] = vertices[1] + (transform.up * ParentBoundary.boundaryHeight);
            vertices[4] = vertices[0] + transform.forward * MyBoxCollider.size.z;
            vertices[5] = vertices[1] + transform.forward * MyBoxCollider.size.z;
            vertices[6] = vertices[4] + transform.up * ParentBoundary.boundaryHeight;
            vertices[7] = vertices[5] + transform.up * ParentBoundary.boundaryHeight;

            return vertices;
        }

        /// <summary>
        /// Some hack-job, hard-coded int array that results in the vertices returned by ReturnVertices() turning into triangles that 
        /// Unity can use to generate a mesh with the faces pointed the correct way. Not my area of expertise, but it works. 
        /// Feel free to contact me @ umbraevolution@gmail.com if you have a better (procedural?) way to do this
        /// </summary>
        /// <returns>Returns an int array used by Unity's Mesh component to turn vertices into renderable triangles</returns>
        public int[] ReturnTriangles()
        {
            return new int[]
            {
                0, 1, 2,
                1, 3, 2,
                1, 0, 4,
                4, 5, 1,
                0, 2, 6,
                6, 4, 0,
                3, 1, 7,
                7, 1, 5,
                7, 6, 3,
                3, 6, 2,
                5, 4, 6,
                6, 7, 5
            };
        }
    }
}
