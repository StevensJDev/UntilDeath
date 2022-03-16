using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections;
using System;
using System.Collections.Generic;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class Kit_PvE_ZombieWaveSurvival_EditorZombieWizard : EditorWindow
        {
            /// <summary>
            /// The animator that will be assigned by the wizard
            /// </summary>
            public AnimatorController zombieAnimator;
            /// <summary>
            /// Assign animator here
            /// </summary>
            public Animator anim;

            string errorString;

            [MenuItem("MMFPSE/Zombies/Setup new zombie model")]
            public static void Open()
            {
                Kit_PvE_ZombieWaveSurvival_EditorZombieWizard window = (Kit_PvE_ZombieWaveSurvival_EditorZombieWizard)EditorWindow.GetWindow(typeof(Kit_PvE_ZombieWaveSurvival_EditorZombieWizard));
                window.Show();
            }

            private void OnGUI()
            {
                GUILayout.Label("Welcome to the zombie model wizard.", EditorStyles.boldLabel);
                if (!zombieAnimator)
                {
                    zombieAnimator = EditorGUILayout.ObjectField("Assign zombie animator:", zombieAnimator, typeof(AnimatorController), false) as AnimatorController;
                }

                errorString = CheckConsistency();
                CalculateAxes();

                anim = EditorGUILayout.ObjectField("Animator: ", anim, typeof(Animator), true) as Animator;

                if (anim)
                {
                    if (GUILayout.Button("Auto Find"))
                    {
                        FindObjects();
                    }
                }

                pelvis = EditorGUILayout.ObjectField("Pelvis: ", pelvis, typeof(Transform), true) as Transform;
                leftHips = EditorGUILayout.ObjectField("Left Hips: ", leftHips, typeof(Transform), true) as Transform;
                leftKnee = EditorGUILayout.ObjectField("Left Knee: ", leftKnee, typeof(Transform), true) as Transform;
                leftFoot = EditorGUILayout.ObjectField("Left Foot: ", leftFoot, typeof(Transform), true) as Transform;
                rightHips = EditorGUILayout.ObjectField("Right Hips: ", rightHips, typeof(Transform), true) as Transform;
                rightKnee = EditorGUILayout.ObjectField("Right Knee: ", rightKnee, typeof(Transform), true) as Transform;
                rightFoot = EditorGUILayout.ObjectField("Right Foot: ", rightFoot, typeof(Transform), true) as Transform;
                leftArm = EditorGUILayout.ObjectField("Left Arm: ", leftArm, typeof(Transform), true) as Transform;
                leftElbow = EditorGUILayout.ObjectField("Left Elbow: ", leftElbow, typeof(Transform), true) as Transform;
                rightArm = EditorGUILayout.ObjectField("Right Arm: ", rightArm, typeof(Transform), true) as Transform;
                rightElbow = EditorGUILayout.ObjectField("Right Elbow: ", rightElbow, typeof(Transform), true) as Transform;
                middleSpine = EditorGUILayout.ObjectField("Middle Spine: ", middleSpine, typeof(Transform), true) as Transform;
                head = EditorGUILayout.ObjectField("Head: ", head, typeof(Transform), true) as Transform; ;

                if (errorString.Length != 0)
                {
                    EditorGUILayout.HelpBox("Drag all bones from the hierarchy into their slots.\nMake sure your character is in T-Stand.\n", MessageType.Error);
                    EditorGUILayout.HelpBox(errorString, MessageType.Error);
                }
                else
                {
                    EditorGUILayout.HelpBox("Make sure your character is in T-Stand.\nMake sure the blue axis faces in the same direction the chracter is looking.", MessageType.Info);

                    if (!anim)
                    {
                        EditorGUILayout.HelpBox("Assign animator!", MessageType.Error);
                    }
                    else
                    {
                        if (GUILayout.Button("Create zombie model"))
                        {
                            Cleanup();
                            BuildCapsules();
                            AddBreastColliders();
                            AddHeadCollider();

                            BuildBodies();
                            BuildJoints();
                            CalculateMass();

                            SetupAnimator();
                            CreateMarsKitModelInHierarchy();
                            CreateVoice();

                            string path = EditorUtility.SaveFilePanel("Save Zombie prefab", Application.dataPath, aiRenderer.name, "prefab");

                            path = path.Replace(Application.dataPath, "");
                            path = "Assets/" + path;

                            if (path.Length > 0)
                            {
                                if (PrefabUtility.GetPrefabAssetType(anim.gameObject) != PrefabAssetType.NotAPrefab)
                                {
                                    if (PrefabUtility.GetPrefabInstanceStatus(anim.gameObject) != PrefabInstanceStatus.NotAPrefab)
                                    {
                                        PrefabUtility.UnpackPrefabInstance(anim.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
                                    }
                                }

#if UNITY_2018_3_OR_NEWER
                                GameObject thirdPersonPrefab = PrefabUtility.SaveAsPrefabAsset(aiRenderer.gameObject, path);
#else
                            GameObject thirdPersonPrefab = PrefabUtility.CreatePrefab(path, aiRenderer.gameObject);
#endif

                                Kit_PvE_ZombieWaveSurvival_ZombieAIRenderer rdnr = thirdPersonPrefab.GetComponent<Kit_PvE_ZombieWaveSurvival_ZombieAIRenderer>();
                                if (thirdPersonPrefab)
                                {
                                    Kit_PvE_ZombieWaveSurvival_ZombieAISettings settings = ScriptableObject.CreateInstance<Kit_PvE_ZombieWaveSurvival_ZombieAISettings>();

                                    string infoPath = EditorUtility.SaveFilePanel("Save Zombie Settings file", Application.dataPath, aiRenderer.name + "_Settings", "asset");

                                    infoPath = infoPath.Replace(Application.dataPath, "");
                                    infoPath = "Assets/" + infoPath;

                                    if (infoPath.Length > 0)
                                    {
                                        AssetDatabase.CreateAsset(settings, infoPath);

                                        Kit_PvE_ZombieWaveSurvival_ZombieAISettings savedSettings = AssetDatabase.LoadAssetAtPath(infoPath, typeof(Kit_PvE_ZombieWaveSurvival_ZombieAISettings)) as Kit_PvE_ZombieWaveSurvival_ZombieAISettings;

                                        rdnr.settings = savedSettings;

                                        EditorUtility.SetDirty(thirdPersonPrefab);
                                        PrefabUtility.RecordPrefabInstancePropertyModifications(thirdPersonPrefab);

                                        EditorUtility.DisplayDialog("Almost done!", "Your prefab and settings have been saved. The settings have no attacks and sounds assigned, so you have to set them up yourself. For reference, see Zombie-Documentation.pdf", "Go on..");

                                        EditorUtility.DisplayDialog("Almost done!", "Now you can assign it to the skin slot in the ZombieWaveSurvival game-mode file. For reference, see Zombie-Documentation.pdf", "Got it mate");

                                        DestroyImmediate(aiRenderer.gameObject);

                                        PrefabUtility.InstantiatePrefab(thirdPersonPrefab);
                                        Close();
                                    }
                                }
                            }

                            AssetDatabase.SaveAssets();
                        }
                    }
                }
            }

            void FindObjects()
            {
                if (!pelvis) pelvis = FindRecursive("Hips");
                if (!pelvis) pelvis = FindRecursive("Pelvis");
                if (!pelvis) pelvis = FindRecursive("pelvis");

                if (!leftHips) leftHips = FindRecursive("LeftUpLeg");
                if (!leftHips) leftHips = FindRecursive("thigh_l");

                if (!leftKnee) leftKnee = FindRecursive("LeftLeg");
                if (!leftKnee) leftKnee = FindRecursive("calf_l");

                if (!leftFoot) leftFoot = FindRecursive("LeftFoot");
                if (!leftFoot) leftFoot = FindRecursive("foot_l");

                if (!rightHips) rightHips = FindRecursive("RightUpLeg");
                if (!rightHips) rightHips = FindRecursive("thigh_r");

                if (!rightKnee) rightKnee = FindRecursive("RightLeg");
                if (!rightKnee) rightKnee = FindRecursive("calf_r");

                if (!rightFoot) rightFoot = FindRecursive("RightFoot");
                if (!rightFoot) rightFoot = FindRecursive("foot_r");

                if (!leftArm) leftArm = FindRecursive("LeftArm");
                if (!leftArm) leftArm = FindRecursive("upperarm_l");

                if (!leftElbow) leftElbow = FindRecursive("LeftForeArm");
                if (!leftElbow) leftElbow = FindRecursive("lowerarm_l");

                if (!rightArm) rightArm = FindRecursive("RightArm");
                if (!rightArm) rightArm = FindRecursive("upperarm_r");

                if (!rightElbow) rightElbow = FindRecursive("RightForeArm");
                if (!rightElbow) rightElbow = FindRecursive("lowerarm_r");

                if (!middleSpine) middleSpine = FindRecursive("Spine1");
                if (!middleSpine) middleSpine = FindRecursive("Spine");
                if (!middleSpine) middleSpine = FindRecursive("spine_01");
                if (!middleSpine) middleSpine = FindRecursive("spine");

                if (!head) head = FindRecursive("Head");
            }

            Transform FindRecursive(string name)
            {
                Transform[] all = anim.transform.GetComponentsInChildren<Transform>();

                for (int i = 0; i < all.Length; i++)
                {
                    if (all[i].name.Contains(name))
                    {
                        return all[i];
                    }
                }

                return null;
            }

            void SetupAnimator()
            {
                anim.runtimeAnimatorController = zombieAnimator as RuntimeAnimatorController;
                anim.applyRootMotion = false;
            }

            Kit_PvE_ZombieWaveSurvival_ZombieAIRenderer aiRenderer;

            void CreateMarsKitModelInHierarchy()
            {
                anim.gameObject.name = "Zombie_" + anim.gameObject.name;
                anim.transform.parent = null;
                //Add Script
                aiRenderer = anim.gameObject.AddComponent<Kit_PvE_ZombieWaveSurvival_ZombieAIRenderer>();
                aiRenderer.anim = anim;
                List<Collider> colliders = new List<Collider>();
                List<Rigidbody> bodies = new List<Rigidbody>();
                int id = 0;
                foreach (BoneInfo bone in bones)
                {
                    Collider col = bone.anchor.GetComponent<Collider>();
                    //Make it a trigger
                    col.isTrigger = true;
                    //Set layer
                    col.gameObject.layer = 9;
                    col.GetComponent<Rigidbody>().isKinematic = true;
                    colliders.Add(col);
                    //Add to script
                    Rigidbody body = col.GetComponent<Rigidbody>();
                    body.interpolation = RigidbodyInterpolation.Interpolate;
                    bodies.Add(body);
                    Kit_PlayerDamageMultiplier pdm = col.gameObject.AddComponent<Kit_PlayerDamageMultiplier>();
                    pdm.ragdollId = id;
                    //Set Tag
                    col.tag = "PlayerCollider";

                    id++;
                }

                //Assign
                aiRenderer.colliders = colliders.ToArray();
                aiRenderer.rigidbodies = bodies.ToArray();

                if (leftFoot && rightFoot)
                {
                    aiRenderer.footSteps = new FootForSound[2];
                    FootForSound left = new FootForSound();
                    left.soundSource = leftFoot.gameObject.AddComponent<AudioSource>();
                    left.soundSource.priority = 256;
                    left.soundSource.spatialBlend = 1f;
                    left.raycastLength = 2f;
                    left.raycastTransform = leftFoot.transform;
                    aiRenderer.footSteps[0] = left;

                    FootForSound right = new FootForSound();
                    right.soundSource = rightFoot.gameObject.AddComponent<AudioSource>();
                    right.soundSource.priority = 256;
                    right.soundSource.spatialBlend = 1f;
                    right.raycastLength = 2f;
                    right.raycastTransform = rightFoot.transform;
                    aiRenderer.footSteps[1] = right;
                }
            }

            void CreateVoice()
            {
                GameObject voiceSound = new GameObject("SoundVoice");
                voiceSound.transform.parent = head.transform;
                voiceSound.transform.localPosition = Vector3.zero;
                aiRenderer.voiceSource = voiceSound.AddComponent<AudioSource>();
                aiRenderer.voiceSource.spatialBlend = 1f;
                aiRenderer.voiceSource.priority = 255;
            }

            public Transform pelvis;

            public Transform leftHips = null;
            public Transform leftKnee = null;
            public Transform leftFoot = null;

            public Transform rightHips = null;
            public Transform rightKnee = null;
            public Transform rightFoot = null;

            public Transform leftArm = null;
            public Transform leftElbow = null;

            public Transform rightArm = null;
            public Transform rightElbow = null;

            public Transform middleSpine = null;
            public Transform head = null;


            public float totalMass = 20;
            public float strength = 0.0F;

            Vector3 right = Vector3.right;
            Vector3 up = Vector3.up;
            Vector3 forward = Vector3.forward;

            Vector3 worldRight = Vector3.right;
            Vector3 worldUp = Vector3.up;
            Vector3 worldForward = Vector3.forward;
            public bool flipForward = false;

            class BoneInfo
            {
                public string name;

                public Transform anchor;
                public CharacterJoint joint;
                public BoneInfo parent;

                public float minLimit;
                public float maxLimit;
                public float swingLimit;

                public Vector3 axis;
                public Vector3 normalAxis;

                public float radiusScale;
                public Type colliderType;

                public ArrayList children = new ArrayList();
                public float density;
                public float summedMass;// The mass of this and all children bodies
            }

            ArrayList bones;
            BoneInfo rootBone;

            string CheckConsistency()
            {
                PrepareBones();
                Hashtable map = new Hashtable();
                foreach (BoneInfo bone in bones)
                {
                    if (bone.anchor)
                    {
                        if (map[bone.anchor] != null)
                        {
                            BoneInfo oldBone = (BoneInfo)map[bone.anchor];
                            return String.Format("{0} and {1} may not be assigned to the same bone.", bone.name, oldBone.name);
                        }
                        map[bone.anchor] = bone;
                    }
                }

                foreach (BoneInfo bone in bones)
                {
                    if (bone.anchor == null)
                        return String.Format("{0} has not been assigned yet.\n", bone.name);
                }

                return "";
            }

            void OnDrawGizmos()
            {
                if (pelvis)
                {
                    Gizmos.color = Color.red; Gizmos.DrawRay(pelvis.position, pelvis.TransformDirection(right));
                    Gizmos.color = Color.green; Gizmos.DrawRay(pelvis.position, pelvis.TransformDirection(up));
                    Gizmos.color = Color.blue; Gizmos.DrawRay(pelvis.position, pelvis.TransformDirection(forward));
                }
            }

            void DecomposeVector(out Vector3 normalCompo, out Vector3 tangentCompo, Vector3 outwardDir, Vector3 outwardNormal)
            {
                outwardNormal = outwardNormal.normalized;
                normalCompo = outwardNormal * Vector3.Dot(outwardDir, outwardNormal);
                tangentCompo = outwardDir - normalCompo;
            }

            void CalculateAxes()
            {
                if (head != null && pelvis != null)
                    up = CalculateDirectionAxis(pelvis.InverseTransformPoint(head.position));
                if (rightElbow != null && pelvis != null)
                {
                    Vector3 removed, temp;
                    DecomposeVector(out temp, out removed, pelvis.InverseTransformPoint(rightElbow.position), up);
                    right = CalculateDirectionAxis(removed);
                }

                forward = Vector3.Cross(right, up);
                if (flipForward)
                    forward = -forward;
            }

            /*
            void OnWizardUpdate()
            {
                errorString = CheckConsistency();
                CalculateAxes();

                if (errorString.Length != 0)
                {
                    helpString = "Drag all bones from the hierarchy into their slots.\nMake sure your character is in T-Stand.\n";
                }
                else
                {
                    helpString = "Make sure your character is in T-Stand.\nMake sure the blue axis faces in the same direction the chracter is looking.\nUse flipForward to flip the direction";
                }

                isValid = errorString.Length == 0;
            }
            */

            void PrepareBones()
            {
                if (pelvis)
                {
                    worldRight = pelvis.TransformDirection(right);
                    worldUp = pelvis.TransformDirection(up);
                    worldForward = pelvis.TransformDirection(forward);
                }

                bones = new ArrayList();

                rootBone = new BoneInfo();
                rootBone.name = "Pelvis";
                rootBone.anchor = pelvis;
                rootBone.parent = null;
                rootBone.density = 2.5F;
                bones.Add(rootBone);

                AddMirroredJoint("Hips", leftHips, rightHips, "Pelvis", worldRight, worldForward, -20, 70, 30, typeof(CapsuleCollider), 0.3F, 1.5F);
                AddMirroredJoint("Knee", leftKnee, rightKnee, "Hips", worldRight, worldForward, -80, 0, 0, typeof(CapsuleCollider), 0.25F, 1.5F);

                AddJoint("Middle Spine", middleSpine, "Pelvis", worldRight, worldForward, -20, 20, 10, null, 1, 2.5F);

                AddMirroredJoint("Arm", leftArm, rightArm, "Middle Spine", worldUp, worldForward, -70, 10, 50, typeof(CapsuleCollider), 0.25F, 1.0F);
                AddMirroredJoint("Elbow", leftElbow, rightElbow, "Arm", worldForward, worldUp, -90, 0, 0, typeof(CapsuleCollider), 0.20F, 1.0F);

                AddJoint("Head", head, "Middle Spine", worldRight, worldForward, -40, 25, 25, null, 1, 1.0F);
            }

            BoneInfo FindBone(string name)
            {
                foreach (BoneInfo bone in bones)
                {
                    if (bone.name == name)
                        return bone;
                }
                return null;
            }

            void AddMirroredJoint(string name, Transform leftAnchor, Transform rightAnchor, string parent, Vector3 worldTwistAxis, Vector3 worldSwingAxis, float minLimit, float maxLimit, float swingLimit, Type colliderType, float radiusScale, float density)
            {
                AddJoint("Left " + name, leftAnchor, parent, worldTwistAxis, worldSwingAxis, minLimit, maxLimit, swingLimit, colliderType, radiusScale, density);
                AddJoint("Right " + name, rightAnchor, parent, worldTwistAxis, worldSwingAxis, minLimit, maxLimit, swingLimit, colliderType, radiusScale, density);
            }

            void AddJoint(string name, Transform anchor, string parent, Vector3 worldTwistAxis, Vector3 worldSwingAxis, float minLimit, float maxLimit, float swingLimit, Type colliderType, float radiusScale, float density)
            {
                BoneInfo bone = new BoneInfo();
                bone.name = name;
                bone.anchor = anchor;
                bone.axis = worldTwistAxis;
                bone.normalAxis = worldSwingAxis;
                bone.minLimit = minLimit;
                bone.maxLimit = maxLimit;
                bone.swingLimit = swingLimit;
                bone.density = density;
                bone.colliderType = colliderType;
                bone.radiusScale = radiusScale;

                if (FindBone(parent) != null)
                    bone.parent = FindBone(parent);
                else if (name.StartsWith("Left"))
                    bone.parent = FindBone("Left " + parent);
                else if (name.StartsWith("Right"))
                    bone.parent = FindBone("Right " + parent);


                bone.parent.children.Add(bone);
                bones.Add(bone);
            }

            void BuildCapsules()
            {
                foreach (BoneInfo bone in bones)
                {
                    if (bone.colliderType != typeof(CapsuleCollider))
                        continue;

                    int direction;
                    float distance;
                    if (bone.children.Count == 1)
                    {
                        BoneInfo childBone = (BoneInfo)bone.children[0];
                        Vector3 endPoint = childBone.anchor.position;
                        CalculateDirection(bone.anchor.InverseTransformPoint(endPoint), out direction, out distance);
                    }
                    else
                    {
                        Vector3 endPoint = (bone.anchor.position - bone.parent.anchor.position) + bone.anchor.position;
                        CalculateDirection(bone.anchor.InverseTransformPoint(endPoint), out direction, out distance);

                        if (bone.anchor.GetComponentsInChildren(typeof(Transform)).Length > 1)
                        {
                            Bounds bounds = new Bounds();
                            foreach (Transform child in bone.anchor.GetComponentsInChildren(typeof(Transform)))
                            {
                                bounds.Encapsulate(bone.anchor.InverseTransformPoint(child.position));
                            }

                            if (distance > 0)
                                distance = bounds.max[direction];
                            else
                                distance = bounds.min[direction];
                        }
                    }

                    CapsuleCollider collider = Undo.AddComponent<CapsuleCollider>(bone.anchor.gameObject);
                    collider.direction = direction;

                    Vector3 center = Vector3.zero;
                    center[direction] = distance * 0.5F;
                    collider.center = center;
                    collider.height = Mathf.Abs(distance);
                    collider.radius = Mathf.Abs(distance * bone.radiusScale);
                }
            }

            void Cleanup()
            {
                foreach (BoneInfo bone in bones)
                {
                    if (!bone.anchor)
                        continue;

                    Component[] joints = bone.anchor.GetComponentsInChildren(typeof(Joint));
                    foreach (Joint joint in joints)
                        Undo.DestroyObjectImmediate(joint);

                    Component[] bodies = bone.anchor.GetComponentsInChildren(typeof(Rigidbody));
                    foreach (Rigidbody body in bodies)
                        Undo.DestroyObjectImmediate(body);

                    Component[] colliders = bone.anchor.GetComponentsInChildren(typeof(Collider));
                    foreach (Collider collider in colliders)
                        Undo.DestroyObjectImmediate(collider);
                }
            }

            void BuildBodies()
            {
                foreach (BoneInfo bone in bones)
                {
                    Undo.AddComponent<Rigidbody>(bone.anchor.gameObject);
                    bone.anchor.GetComponent<Rigidbody>().mass = bone.density;
                }
            }

            void BuildJoints()
            {
                foreach (BoneInfo bone in bones)
                {
                    if (bone.parent == null)
                        continue;

                    CharacterJoint joint = Undo.AddComponent<CharacterJoint>(bone.anchor.gameObject);
                    bone.joint = joint;

                    // Setup connection and axis
                    joint.axis = CalculateDirectionAxis(bone.anchor.InverseTransformDirection(bone.axis));
                    joint.swingAxis = CalculateDirectionAxis(bone.anchor.InverseTransformDirection(bone.normalAxis));
                    joint.anchor = Vector3.zero;
                    joint.connectedBody = bone.parent.anchor.GetComponent<Rigidbody>();
                    joint.enablePreprocessing = false; // turn off to handle degenerated scenarios, like spawning inside geometry.

                    // Setup limits
                    SoftJointLimit limit = new SoftJointLimit();
                    limit.contactDistance = 0; // default to zero, which automatically sets contact distance.

                    limit.limit = bone.minLimit;
                    joint.lowTwistLimit = limit;

                    limit.limit = bone.maxLimit;
                    joint.highTwistLimit = limit;

                    limit.limit = bone.swingLimit;
                    joint.swing1Limit = limit;

                    limit.limit = 0;
                    joint.swing2Limit = limit;
                }
            }

            void CalculateMassRecurse(BoneInfo bone)
            {
                float mass = bone.anchor.GetComponent<Rigidbody>().mass;
                foreach (BoneInfo child in bone.children)
                {
                    CalculateMassRecurse(child);
                    mass += child.summedMass;
                }
                bone.summedMass = mass;
            }

            void CalculateMass()
            {
                // Calculate allChildMass by summing all bodies
                CalculateMassRecurse(rootBone);

                // Rescale the mass so that the whole character weights totalMass
                float massScale = totalMass / rootBone.summedMass;
                foreach (BoneInfo bone in bones)
                    bone.anchor.GetComponent<Rigidbody>().mass *= massScale;

                // Recalculate allChildMass by summing all bodies
                CalculateMassRecurse(rootBone);
            }

            static void CalculateDirection(Vector3 point, out int direction, out float distance)
            {
                // Calculate longest axis
                direction = 0;
                if (Mathf.Abs(point[1]) > Mathf.Abs(point[0]))
                    direction = 1;
                if (Mathf.Abs(point[2]) > Mathf.Abs(point[direction]))
                    direction = 2;

                distance = point[direction];
            }

            static Vector3 CalculateDirectionAxis(Vector3 point)
            {
                int direction = 0;
                float distance;
                CalculateDirection(point, out direction, out distance);
                Vector3 axis = Vector3.zero;
                if (distance > 0)
                    axis[direction] = 1.0F;
                else
                    axis[direction] = -1.0F;
                return axis;
            }

            static int SmallestComponent(Vector3 point)
            {
                int direction = 0;
                if (Mathf.Abs(point[1]) < Mathf.Abs(point[0]))
                    direction = 1;
                if (Mathf.Abs(point[2]) < Mathf.Abs(point[direction]))
                    direction = 2;
                return direction;
            }

            static int LargestComponent(Vector3 point)
            {
                int direction = 0;
                if (Mathf.Abs(point[1]) > Mathf.Abs(point[0]))
                    direction = 1;
                if (Mathf.Abs(point[2]) > Mathf.Abs(point[direction]))
                    direction = 2;
                return direction;
            }

            static int SecondLargestComponent(Vector3 point)
            {
                int smallest = SmallestComponent(point);
                int largest = LargestComponent(point);
                if (smallest < largest)
                {
                    int temp = largest;
                    largest = smallest;
                    smallest = temp;
                }

                if (smallest == 0 && largest == 1)
                    return 2;
                else if (smallest == 0 && largest == 2)
                    return 1;
                else
                    return 0;
            }

            Bounds Clip(Bounds bounds, Transform relativeTo, Transform clipTransform, bool below)
            {
                int axis = LargestComponent(bounds.size);

                if (Vector3.Dot(worldUp, relativeTo.TransformPoint(bounds.max)) > Vector3.Dot(worldUp, relativeTo.TransformPoint(bounds.min)) == below)
                {
                    Vector3 min = bounds.min;
                    min[axis] = relativeTo.InverseTransformPoint(clipTransform.position)[axis];
                    bounds.min = min;
                }
                else
                {
                    Vector3 max = bounds.max;
                    max[axis] = relativeTo.InverseTransformPoint(clipTransform.position)[axis];
                    bounds.max = max;
                }
                return bounds;
            }

            Bounds GetBreastBounds(Transform relativeTo)
            {
                // Pelvis bounds
                Bounds bounds = new Bounds();
                bounds.Encapsulate(relativeTo.InverseTransformPoint(leftHips.position));
                bounds.Encapsulate(relativeTo.InverseTransformPoint(rightHips.position));
                bounds.Encapsulate(relativeTo.InverseTransformPoint(leftArm.position));
                bounds.Encapsulate(relativeTo.InverseTransformPoint(rightArm.position));
                Vector3 size = bounds.size;
                size[SmallestComponent(bounds.size)] = size[LargestComponent(bounds.size)] / 2.0F;
                bounds.size = size;
                return bounds;
            }

            void AddBreastColliders()
            {
                // Middle spine and pelvis
                if (middleSpine != null && pelvis != null)
                {
                    Bounds bounds;
                    BoxCollider box;

                    // Middle spine bounds
                    bounds = Clip(GetBreastBounds(pelvis), pelvis, middleSpine, false);
                    box = Undo.AddComponent<BoxCollider>(pelvis.gameObject);
                    box.center = bounds.center;
                    box.size = bounds.size;

                    bounds = Clip(GetBreastBounds(middleSpine), middleSpine, middleSpine, true);
                    box = Undo.AddComponent<BoxCollider>(middleSpine.gameObject);
                    box.center = bounds.center;
                    box.size = bounds.size;
                }
                // Only pelvis
                else
                {
                    Bounds bounds = new Bounds();
                    bounds.Encapsulate(pelvis.InverseTransformPoint(leftHips.position));
                    bounds.Encapsulate(pelvis.InverseTransformPoint(rightHips.position));
                    bounds.Encapsulate(pelvis.InverseTransformPoint(leftArm.position));
                    bounds.Encapsulate(pelvis.InverseTransformPoint(rightArm.position));

                    Vector3 size = bounds.size;
                    size[SmallestComponent(bounds.size)] = size[LargestComponent(bounds.size)] / 2.0F;

                    BoxCollider box = Undo.AddComponent<BoxCollider>(pelvis.gameObject);
                    box.center = bounds.center;
                    box.size = size;
                }
            }

            void AddHeadCollider()
            {
                if (head.GetComponent<Collider>())
                    Destroy(head.GetComponent<Collider>());

                float radius = Vector3.Distance(leftArm.transform.position, rightArm.transform.position);
                radius /= 4;

                SphereCollider sphere = Undo.AddComponent<SphereCollider>(head.gameObject);
                sphere.radius = radius;
                Vector3 center = Vector3.zero;

                int direction;
                float distance;
                CalculateDirection(head.InverseTransformPoint(pelvis.position), out direction, out distance);
                if (distance > 0)
                    center[direction] = -radius;
                else
                    center[direction] = radius;
                sphere.center = center;
            }
        }
    }
}
