using UnityEngine;
//using UnityEditor;
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using SharpKit.JavaScript;

public class JSBindingSettings
{
	public static string jsExtension = ".javascript";
	public static string jsDir = Application.streamingAssetsPath + "/JavaScript";
	public static string jsRelDir = "Assets/StreamingAssets/JavaScript";
	
	public static string jsGenDir { get { return jsDir + "/Generated"; } }
	public static string jsGenFiles { get { return jsDir + "/GeneratedFiles" + jsExtension; } }
	public static string csGenDir = Application.dataPath + "/JSBinding/Generated";

    public static Type[] enums = new Type[]
    {
		typeof(KeyCode),
    };
	    
    public static Type[] classes = new Type[]
    {
		typeof(UnityEngine.RectTransform),
		typeof(UnityEngine.EventSystems.EventTrigger),

		typeof(System.Random),
		typeof(RectTransformUtility),

		typeof(System.IO.Path),
		typeof(System.IO.Directory),
		typeof(System.IO.File),
		typeof(System.IO.DirectoryInfo),
		typeof(System.IO.FileSystemInfo),
		typeof(System.MarshalByRefObject),
		typeof(UnityEngine.ICanvasRaycastFilter),
		typeof(UnityEngine.CanvasGroup),
		

#region
        typeof(System.Collections.IEnumerator),
#endregion

		typeof(System.Math),
		typeof(System.TimeSpan),
		typeof(System.DateTime),

		typeof(System.IConvertible),
		//typeof(System.Xml.IHasXmlChildNode),
		typeof(System.Collections.ICollection),
		typeof(System.Runtime.Serialization.ISerializable),
		typeof(System.Runtime.InteropServices._Exception),
		typeof(System.IDisposable),
		typeof(System.Runtime.Serialization.ISurrogateSelector),
		typeof(UnityEngine.ISerializationCallbackReceiver),
//		typeof(UnityEngine.UI.IGraphicEnabledDisabled),
		//typeof(UnityEngine.UI.IGraphicEnabledDisabled),
		//typeof(IEquatable<UnityEngine.UI.LayoutRebuilder>),

        typeof(System.Xml.XmlNode),
		typeof(System.Xml.XPath.IXPathNavigable),
		typeof(System.ICloneable),
		typeof(System.Collections.IEnumerable),
        typeof(System.Xml.XmlDocument),
        typeof(System.Xml.XmlNodeList),
        typeof(System.Xml.XmlElement),
        typeof(System.Xml.XmlLinkedNode),
        typeof(System.Xml.XmlAttributeCollection),
        typeof(System.Xml.XmlNamedNodeMap),
        typeof(System.Xml.XmlAttribute),

        typeof(UnityEngine.Security),

//        typeof(UnityEngine.StackTraceUtility),

        typeof(UnityEngine.UnityException),
        typeof(UnityEngine.MissingComponentException),
        typeof(UnityEngine.UnassignedReferenceException),
        typeof(UnityEngine.MissingReferenceException),
        typeof(UnityEngine.TextEditor),

        typeof(UnityEngine.TextGenerator),
        typeof(UnityEngine.TrackedReference),
        typeof(UnityEngine.WWW),


        typeof(UnityEngine.Serialization.UnitySurrogateSelector),
//        typeof(UnityEngineInternal.GenericStack),
        typeof(UnityEngine.Physics),
        typeof(UnityEngine.Rigidbody),
        typeof(UnityEngine.Joint),
        typeof(UnityEngine.HingeJoint),
        typeof(UnityEngine.SpringJoint),
        typeof(UnityEngine.FixedJoint),
        typeof(UnityEngine.CharacterJoint),
        typeof(UnityEngine.ConfigurableJoint),
        typeof(UnityEngine.ConstantForce),
        typeof(UnityEngine.Collider),
        typeof(UnityEngine.BoxCollider),
        typeof(UnityEngine.SphereCollider),
        typeof(UnityEngine.MeshCollider),
        typeof(UnityEngine.CapsuleCollider),
        
        typeof(UnityEngine.WheelCollider),
        typeof(UnityEngine.PhysicMaterial),
        typeof(UnityEngine.Collision),
        typeof(UnityEngine.ControllerColliderHit),
        typeof(UnityEngine.CharacterController),
        typeof(UnityEngine.Cloth),
        //typeof(UnityEngine.InteractiveCloth),
        //typeof(UnityEngine.SkinnedCloth),
        //typeof(UnityEngine.ClothRenderer),
        typeof(UnityEngine.TerrainCollider),
        typeof(UnityEngine.Physics2D),
        typeof(UnityEngine.Rigidbody2D),
        typeof(UnityEngine.Collider2D),
        typeof(UnityEngine.CircleCollider2D),
        typeof(UnityEngine.BoxCollider2D),
        typeof(UnityEngine.EdgeCollider2D),
        typeof(UnityEngine.PolygonCollider2D),
        typeof(UnityEngine.Collision2D),
        typeof(UnityEngine.Joint2D),
        typeof(UnityEngine.AnchoredJoint2D),
        typeof(UnityEngine.SpringJoint2D),
        typeof(UnityEngine.DistanceJoint2D),
        typeof(UnityEngine.HingeJoint2D),

        typeof(UnityEngine.SliderJoint2D),
        typeof(UnityEngine.WheelJoint2D),
        typeof(UnityEngine.PhysicsMaterial2D),
        typeof(UnityEngine.NavMeshAgent),
        typeof(UnityEngine.NavMesh),
        typeof(UnityEngine.OffMeshLink),
        typeof(UnityEngine.NavMeshPath),
        typeof(UnityEngine.NavMeshObstacle),
        typeof(UnityEngine.AudioSettings),

        typeof(UnityEngine.AudioClip),
        typeof(UnityEngine.AudioListener),
        typeof(UnityEngine.AudioSource),
        typeof(UnityEngine.AudioReverbZone),
        typeof(UnityEngine.AudioLowPassFilter),

        typeof(UnityEngine.AudioHighPassFilter),
        typeof(UnityEngine.AudioDistortionFilter),
        typeof(UnityEngine.AudioEchoFilter),
        typeof(UnityEngine.AudioChorusFilter),
        typeof(UnityEngine.AudioReverbFilter),
        typeof(UnityEngine.Microphone),
        typeof(UnityEngine.WebCamTexture),
        typeof(UnityEngine.AnimationClipPair),
//        typeof(UnityEngine.AnimatorOverrideController),
        typeof(UnityEngine.AnimationEvent),
        typeof(UnityEngine.AnimationClip),
        typeof(UnityEngine.AnimationCurve),
        typeof(UnityEngine.Animation),
        typeof(UnityEngine.AnimationState),
        typeof(UnityEngine.GameObject),
        typeof(UnityEngine.Animator),
        typeof(UnityEngine.AvatarBuilder),
        typeof(UnityEngine.RuntimeAnimatorController),
		typeof(UnityEngine.AnimatorOverrideController),
        typeof(UnityEngine.Avatar),
        typeof(UnityEngine.HumanTrait),
        typeof(UnityEngine.TreePrototype),
        typeof(UnityEngine.DetailPrototype),
        typeof(UnityEngine.SplatPrototype),
//        typeof(UnityEngine.TerrainData),
//        typeof(UnityEngine.Terrain),
//        typeof(UnityEngine.Tree),                                              
        typeof(UnityEngine.AssetBundleCreateRequest),                          
        typeof(UnityEngine.AssetBundleRequest),                                
        typeof(UnityEngine.AssetBundle),                                       
        typeof(UnityEngine.SystemInfo),                                        
        typeof(UnityEngine.WaitForSeconds),                                    
        typeof(UnityEngine.WaitForFixedUpdate),                                
        typeof(UnityEngine.WaitForEndOfFrame),                                 
        typeof(UnityEngine.Coroutine),     
        // attributes                            
        // typeof(UnityEngine.DisallowMultipleComponent),                         
        //typeof(UnityEngine.RequireComponent),                                  
        //typeof(UnityEngine.AddComponentMenu),                                  
        //typeof(UnityEngine.ContextMenu),                                       
        //typeof(UnityEngine.ExecuteInEditMode),                                 
        //typeof(UnityEngine.HideInInspector),                                   
        typeof(UnityEngine.ScriptableObject),                                  
        typeof(UnityEngine.Resources),                                         
        typeof(UnityEngine.Profiler),                                          
        // typeof(UnityEngineInternal.Reproduction),                              
        typeof(UnityEngine.CrashReport),                                       
        typeof(UnityEngine.Cursor),                                            
        typeof(UnityEngine.OcclusionArea),                                     
        typeof(UnityEngine.OcclusionPortal),                                   
        typeof(UnityEngine.RenderSettings),                                    
        typeof(UnityEngine.QualitySettings),                                   
        typeof(UnityEngine.MeshFilter),                                        
        typeof(UnityEngine.Mesh),                                              
        typeof(UnityEngine.SkinnedMeshRenderer),                               
        typeof(UnityEngine.Flare),                                             
        typeof(UnityEngine.LensFlare),                                         
        typeof(UnityEngine.Renderer),                                          
        typeof(UnityEngine.Projector),                                         
        typeof(UnityEngine.Skybox),                                            
        typeof(UnityEngine.TextMesh),                                          
        typeof(UnityEngine.ParticleEmitter),                                   
        typeof(UnityEngine.ParticleAnimator),                                  
        typeof(UnityEngine.TrailRenderer),                                     
        typeof(UnityEngine.ParticleRenderer),                                  
        typeof(UnityEngine.LineRenderer),                                      
        typeof(UnityEngine.MaterialPropertyBlock),                             
        typeof(UnityEngine.Graphics),                                          
        typeof(UnityEngine.LightmapData),                                      
        typeof(UnityEngine.LightProbes),                                       
        typeof(UnityEngine.LightmapSettings),                                  
        typeof(UnityEngine.GeometryUtility),                                   
        typeof(UnityEngine.Screen),                                            
        typeof(UnityEngine.SleepTimeout),                                      
        typeof(UnityEngine.GL),                                                
        typeof(UnityEngine.MeshRenderer),                                      
        typeof(UnityEngine.StaticBatchingUtility),                             
        //typeof(UnityEngine.ImageEffectTransformsToLDR),                        
        //typeof(UnityEngine.ImageEffectOpaque),                                 
        typeof(UnityEngine.Texture),                                           
        typeof(UnityEngine.Texture2D),                                         
        typeof(UnityEngine.Cubemap),                                           
        typeof(UnityEngine.Texture3D),                                         
        typeof(UnityEngine.SparseTexture),                                     
        typeof(UnityEngine.RenderTexture),                                     
        typeof(UnityEngine.GUIElement),                                        
        typeof(UnityEngine.GUITexture),                                        
        typeof(UnityEngine.GUIText),                                           
        typeof(UnityEngine.Font),                                              
        typeof(UnityEngine.GUILayer),                                          
        typeof(UnityEngine.LODGroup),                                          
        typeof(UnityEngine.Gradient),                                          
        typeof(UnityEngine.GUI),
        typeof(UnityEngine.GUILayout),
        typeof(UnityEngine.GUILayoutUtility),                                    
        typeof(UnityEngine.GUILayoutOption),                                     
        typeof(UnityEngine.ExitGUIException),
        typeof(UnityEngine.GUIUtility),
        typeof(UnityEngine.GUISettings),
        typeof(UnityEngine.GUISkin),
        typeof(UnityEngine.GUIContent),
        typeof(UnityEngine.GUIStyleState),
        typeof(UnityEngine.RectOffset),
        typeof(UnityEngine.GUIStyle),                     
        typeof(UnityEngine.Event),                                   
        typeof(UnityEngine.Gizmos),                    
        typeof(UnityEngine.LightProbeGroup),                         

        
//         typeof(UnityEngine.Ping),                                    
//         typeof(UnityEngine.NetworkView),                             
//         typeof(UnityEngine.Network),                                 
//         typeof(UnityEngine.BitStream),                               
//         //typeof(UnityEngine.RPC),                                     
//         typeof(UnityEngine.HostData),                                
//         typeof(UnityEngine.MasterServer),                            
        

        typeof(UnityEngine.ParticleSystem),                          
        typeof(UnityEngine.ParticleSystemRenderer),                  
        typeof(UnityEngine.TextAsset),                               
                    
        //typeof(UnityEngine.SerializeField),                          
        typeof(UnityEngine.Shader),                                  
        typeof(UnityEngine.Material),                                
        typeof(UnityEngine.ProceduralPropertyDescription),           
        typeof(UnityEngine.ProceduralMaterial),                      
        typeof(UnityEngine.ProceduralTexture),                       
        typeof(UnityEngine.Sprite),                                  
        typeof(UnityEngine.SpriteRenderer),                          
        typeof(UnityEngine.Sprites.DataUtility),                     
        typeof(UnityEngine.WWWForm),                                 
        typeof(UnityEngine.Caching),                                 
        typeof(UnityEngine.AsyncOperation),                          
        typeof(UnityEngine.Application),                             
        typeof(UnityEngine.Behaviour),                               
        typeof(UnityEngine.Camera),                                  
        typeof(UnityEngine.ComputeShader),                           
        typeof(UnityEngine.ComputeBuffer),                           
        typeof(UnityEngine.Debug),                                   
        typeof(UnityEngine.Display),
        //typeof(UnityEngine.Flash.ActionScript),                      
        //typeof(UnityEngine.Flash.FlashPlayer),                       
        typeof(UnityEngine.MonoBehaviour),                           
        typeof(UnityEngine.Gyroscope),                               
        typeof(UnityEngine.LocationService),                         
        typeof(UnityEngine.Compass),                                 
        typeof(UnityEngine.Input),                                   
        typeof(UnityEngine.Object),
        typeof(UnityEngine.Component),                              
        typeof(UnityEngine.Light),                                  
        typeof(UnityEngine.Transform),                              
        typeof(UnityEngine.Time),                                   
        typeof(UnityEngine.Random),
        typeof(UnityEngine.YieldInstruction),
        typeof(UnityEngine.PlayerPrefsException),
        typeof(UnityEngine.PlayerPrefs),                               
        
         //ValueType

        typeof(UnityEngine.SocialPlatforms.Range),             
        typeof(UnityEngine.TextGenerationSettings),            
        typeof(UnityEngine.JointMotor),                        
        typeof(UnityEngine.JointSpring),                       
        typeof(UnityEngine.JointLimits),                       
        typeof(UnityEngine.SoftJointLimit),                    
        typeof(UnityEngine.JointDrive),                        
        typeof(UnityEngine.WheelFrictionCurve),                
        typeof(UnityEngine.WheelHit),                          
        typeof(UnityEngine.RaycastHit),                        
        typeof(UnityEngine.ContactPoint),                      
        typeof(UnityEngine.ClothSkinningCoefficient),          
        typeof(UnityEngine.RaycastHit2D),                      
        typeof(UnityEngine.ContactPoint2D),                    
        typeof(UnityEngine.JointAngleLimits2D),                
        typeof(UnityEngine.JointTranslationLimits2D),          
        typeof(UnityEngine.JointMotor2D),                      
        typeof(UnityEngine.JointSuspension2D),                 
        typeof(UnityEngine.OffMeshLinkData),                
        typeof(UnityEngine.NavMeshHit),                     
        typeof(UnityEngine.NavMeshTriangulation),           
        typeof(UnityEngine.WebCamDevice),                   
        typeof(UnityEngine.Keyframe),   
#if UNITY_5                    
        typeof(AnimatorClipInfo),                  
#else
        typeof(UnityEngine.AnimationInfo),  
#endif		
        typeof(UnityEngine.AnimatorStateInfo),              
        typeof(UnityEngine.AnimatorTransitionInfo),         
        typeof(UnityEngine.MatchTargetWeightMask),          
        typeof(UnityEngine.SkeletonBone),                   
        typeof(UnityEngine.HumanLimit),                     
        typeof(UnityEngine.HumanBone),                      
        typeof(UnityEngine.HumanDescription),               
        typeof(UnityEngine.TreeInstance),                   
        typeof(UnityEngine.UIVertex),                       
        typeof(UnityEngine.LayerMask),                      
        typeof(UnityEngine.CombineInstance),                
        typeof(UnityEngine.BoneWeight),                     
        typeof(UnityEngine.Particle),                       
        typeof(UnityEngine.RenderBuffer),                   
        typeof(UnityEngine.Resolution),                     
        typeof(UnityEngine.CharacterInfo),                  
        typeof(UnityEngine.UICharInfo),                     
        typeof(UnityEngine.UILineInfo),                     
        typeof(UnityEngine.LOD),                            
        typeof(UnityEngine.GradientColorKey),               
        typeof(UnityEngine.GradientAlphaKey),                 
        typeof(UnityEngine.Vector2),                        
        typeof(UnityEngine.Vector3),                    
        typeof(UnityEngine.Color),                       
        typeof(UnityEngine.Color32),                     
        typeof(UnityEngine.Quaternion),                  
        typeof(UnityEngine.Rect),                        
        typeof(UnityEngine.Matrix4x4),                   
        typeof(UnityEngine.Bounds),                      
        typeof(UnityEngine.Vector4),                     
        typeof(UnityEngine.Ray),                         
        typeof(UnityEngine.Ray2D),                       
        typeof(UnityEngine.Plane),                       
        typeof(UnityEngine.Mathf),         
        
        
//         typeof(UnityEngine.NetworkPlayer),               
//         typeof(UnityEngine.NetworkViewID),               
//         typeof(UnityEngine.NetworkMessageInfo),          
        
        typeof(UnityEngine.Touch),                       
        typeof(UnityEngine.AccelerationEvent),           
        typeof(UnityEngine.LocationInfo), 
   
        //////////////////////////////////////////////////////
        // types not from UnityEngine
        typeof(System.Diagnostics.Stopwatch),

#if (UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN) && (!UNITY_IPHONE && !UNITY_ANDROID)
        typeof(UnityEngine.MovieTexture),
#endif

        //////////////////////////////////////////////////////
        // iPhone only
#if UNITY_IPHONE
                         
                                          
//         typeof(iPhoneInput),                             
//         typeof(UnityEngine.iPhoneSettings),                          
//                                   
//         typeof(UnityEngine.iPhoneUtils),                             
//         typeof(UnityEngine.LocalNotification),                       
//         typeof(UnityEngine.RemoteNotification),                      
//         typeof(UnityEngine.NotificationServices),
//         typeof(UnityEngine.Device),
//         typeof(UnityEngine.ADBannerView),
//         typeof(UnityEngine.ADInterstitialAd),
#endif

        //////////////////////////////////////////////////////
        // Android only
#if UNITY_ANDROID
        typeof(UnityEngine.AndroidJNIHelper),                         
        typeof(UnityEngine.AndroidJNI),                               
        typeof(UnityEngine.AndroidInput),
        typeof(UnityEngine.AndroidJavaException), 
        typeof(UnityEngine.AndroidJavaProxy),
        typeof(UnityEngine.AndroidJavaObject),
        typeof(UnityEngine.AndroidJavaClass),
#endif
        //////////////////////////////////////////////////////
        // iPhone and Android
#if UNITY_ANDROID || UNITY_IPHONE
        typeof(UnityEngine.TouchScreenKeyboard),
#endif
        //////////////////////////////////////////////////////
        //
        // Obsolete!!
        //
//         typeof(UnityEngine.RaycastCollider),
//         typeof(UnityEngine.SerializePrivateVariables),   
//         typeof(UnityEngine.CacheIndex),                  
//         typeof(UnityEngine.iPhoneTouch),   
//         typeof(UnityEngine.iPhoneAccelerationEvent), 
//         typeof(UnityEngine.iPhoneKeyboard),



        //////////////////////////////////////////////////////
        // attributes
//         typeof(AOT.MonoPInvokeCallbackAttribute),
//         typeof(UnityEngine.PropertyAttribute),
//         typeof(UnityEngine.ContextMenuItemAttribute),
//         typeof(UnityEngine.TooltipAttribute),
//         typeof(UnityEngine.SpaceAttribute),
//         typeof(UnityEngine.HeaderAttribute),
//         typeof(UnityEngine.RangeAttribute),
//         typeof(UnityEngine.MultilineAttribute),
//         typeof(UnityEngine.TextAreaAttribute),
//         typeof(UnityEngine.ThreadSafeAttribute),
//         typeof(UnityEngine.ConstructorSafeAttribute),
//         typeof(UnityEngine.AssemblyIsEditorAssembly),        
//         typeof(UnityEngine.ImplementedInActionScriptAttribute),
//         typeof(UnityEngine.SelectionBaseAttribute),
//         typeof(UnityEngineInternal.TypeInferenceRuleAttribute),
//         typeof(UnityEngine.Internal.DefaultValueAttribute),
//         typeof(UnityEngine.Internal.ExcludeFromDocsAttribute),                   
//         typeof(UnityEngine.NotConvertedAttribute),                   
//         typeof(UnityEngine.NotFlashValidatedAttribute),              
//         typeof(UnityEngine.NotRenamedAttribute),  

        // typeof(UnityEngine.SocialPlatforms.GameCenter.GameCenterPlatform), //Action<>..... oh, manually add a namespace can solve this

        //////////////////////////////////////////////////////
        // delegates
        // typeof(UnityEngine.AndroidJavaRunnable),
        // typeof(UnityEngine.Events.UnityAction),
        // typeof(UnityEngineInternal.FastCallExceptionHandler),
        // typeof(UnityEngineInternal.GetMethodDelegate),

        //////////////////////////////////////////////////////
        // static classes
        // typeof(UnityEngine.Types),
        // typeof(UnityEngine.Social),

        //////////////////////////////////////////////////////
        // not exist (why? see IsDiscard)
        typeof(UnityEngine.Motion),

        //typeof(UnityEngine.SamsungTV),









        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //
        // Unity 4.6.2
        //
        //
        //

//#if UNITY_4_6
        
        // interface

        typeof(UnityEngine.EventSystems.IEventSystemHandler),
        typeof(UnityEngine.EventSystems.IPointerEnterHandler),
        typeof(UnityEngine.EventSystems.IPointerExitHandler),
        typeof(UnityEngine.EventSystems.IPointerDownHandler),
        typeof(UnityEngine.EventSystems.IPointerUpHandler),
        typeof(UnityEngine.EventSystems.IPointerClickHandler),
        typeof(UnityEngine.EventSystems.IBeginDragHandler),
        typeof(UnityEngine.EventSystems.IInitializePotentialDragHandler),
        typeof(UnityEngine.EventSystems.IDragHandler),
        typeof(UnityEngine.EventSystems.IEndDragHandler),
        typeof(UnityEngine.EventSystems.IDropHandler),
        typeof(UnityEngine.EventSystems.IScrollHandler),
        typeof(UnityEngine.EventSystems.IUpdateSelectedHandler),
        typeof(UnityEngine.EventSystems.ISelectHandler),
        typeof(UnityEngine.EventSystems.IDeselectHandler),
        typeof(UnityEngine.EventSystems.IMoveHandler),
        typeof(UnityEngine.EventSystems.ISubmitHandler),
        typeof(UnityEngine.EventSystems.ICancelHandler),
        typeof(UnityEngine.UI.ICanvasElement),
        typeof(UnityEngine.UI.IMask),
        typeof(UnityEngine.UI.IMaskable),
        typeof(UnityEngine.UI.ILayoutElement),
        typeof(UnityEngine.UI.ILayoutController),
        typeof(UnityEngine.UI.ILayoutGroup),
        typeof(UnityEngine.UI.ILayoutSelfController),
        typeof(UnityEngine.UI.ILayoutIgnorer),
        typeof(UnityEngine.UI.IMaterialModifier),
        typeof(UnityEngine.UI.IVertexModifier),

        // class

        typeof(UnityEngine.EventSystems.EventSystem),
//        typeof(UnityEngine.EventSystems.EventTrigger),
        typeof(UnityEngine.EventSystems.EventTrigger.TriggerEvent),
//        typeof(UnityEngine.EventSystems.EventTrigger.Entry),
//        typeof(UnityEngine.EventSystems.ExecuteEvents),
        typeof(UnityEngine.EventSystems.UIBehaviour),
        typeof(UnityEngine.EventSystems.AxisEventData),
        typeof(UnityEngine.EventSystems.BaseEventData),
        typeof(UnityEngine.EventSystems.PointerEventData),
        typeof(UnityEngine.EventSystems.BaseInputModule),
        typeof(UnityEngine.EventSystems.PointerInputModule),
//        typeof(UnityEngine.EventSystems.PointerInputModule.MouseButtonEventData),
        typeof(UnityEngine.EventSystems.StandaloneInputModule),
        typeof(UnityEngine.EventSystems.TouchInputModule),
        typeof(UnityEngine.EventSystems.BaseRaycaster),
        typeof(UnityEngine.EventSystems.Physics2DRaycaster),
        typeof(UnityEngine.EventSystems.PhysicsRaycaster),
        //typeof(UnityEngine.UI.CoroutineTween.ColorTween.ColorTweenCallback),
        typeof(UnityEngine.UI.AnimationTriggers),
        typeof(UnityEngine.UI.Button),
        typeof(UnityEngine.UI.Button.ButtonClickedEvent),
        typeof(UnityEngine.UI.CanvasUpdateRegistry),
        //typeof(UnityEngine.UI.FontUpdateTracker),
        typeof(UnityEngine.UI.Graphic),
        typeof(UnityEngine.UI.GraphicRaycaster),
        //typeof(UnityEngine.UI.GraphicRebuildTracker),
        typeof(UnityEngine.UI.GraphicRegistry),
        typeof(UnityEngine.UI.Image),
        typeof(UnityEngine.UI.InputField),
        typeof(UnityEngine.UI.InputField.SubmitEvent),
        typeof(UnityEngine.UI.InputField.OnChangeEvent),
        typeof(UnityEngine.UI.MaskableGraphic),
        typeof(UnityEngine.UI.RawImage),
        typeof(UnityEngine.UI.Scrollbar),
        typeof(UnityEngine.UI.Scrollbar.ScrollEvent),
        typeof(UnityEngine.UI.ScrollRect),
        typeof(UnityEngine.UI.ScrollRect.ScrollRectEvent),
        typeof(UnityEngine.UI.Selectable),
        typeof(UnityEngine.UI.Slider),
        typeof(UnityEngine.UI.Slider.SliderEvent),
        typeof(UnityEngine.UI.StencilMaterial),
        typeof(UnityEngine.UI.Text),
        typeof(UnityEngine.UI.Toggle),
        typeof(UnityEngine.UI.Toggle.ToggleEvent),
        typeof(UnityEngine.UI.ToggleGroup),
        typeof(UnityEngine.UI.AspectRatioFitter),
        typeof(UnityEngine.UI.CanvasScaler),
        typeof(UnityEngine.UI.ContentSizeFitter),
        typeof(UnityEngine.UI.GridLayoutGroup),
        typeof(UnityEngine.UI.HorizontalLayoutGroup),
        typeof(UnityEngine.UI.HorizontalOrVerticalLayoutGroup),
        typeof(UnityEngine.UI.LayoutElement),
        typeof(UnityEngine.UI.LayoutGroup),
        typeof(UnityEngine.UI.LayoutUtility),
        typeof(UnityEngine.UI.VerticalLayoutGroup),
        typeof(UnityEngine.UI.Mask),
        typeof(UnityEngine.UI.BaseVertexEffect),
        typeof(UnityEngine.UI.Outline),
        typeof(UnityEngine.UI.PositionAsUV1),
        typeof(UnityEngine.UI.Shadow),
        // typeof(UnityEngine.EventSystems.ExecuteEvents.EventFunction`1[T1]),
        

        // ValueType

        typeof(UnityEngine.EventSystems.RaycastResult),
        typeof(UnityEngine.UI.ColorBlock),
        typeof(UnityEngine.UI.FontData),
        typeof(UnityEngine.UI.Navigation),
        typeof(UnityEngine.UI.SpriteState),
        typeof(UnityEngine.UI.LayoutRebuilder),

        //////////////////////////////////////////////////////
        // delegates
        // typeof(UnityEngine.UI.InputField.OnValidateInput),

//#endif // #if UNITY_4_6 || UNITY_4_7

        
        // test
        //typeof(List<>), 
        //typeof(List<>.Enumerator), 

        //typeof(Dictionary<,>), 
        //typeof(Dictionary<,>.KeyCollection), 
        //typeof(Dictionary<,>.ValueCollection), 
        //typeof(Dictionary<,>.ValueCollection.Enumerator),
        //typeof(Dictionary<,>.Enumerator), 
        //typeof(KeyValuePair<,>), 

        //typeof(QiucwCup<>),

        

//        typeof(System.Delegate),
//        typeof(System.MulticastDelegate),
        //typeof(SerializeStruct.AppleInfo),
        typeof(StringBuilder),

//#if UNITY_4_6 || UNITY_4_7
        //typeof(UnityEngine.EventSystems.UIBehaviour),
        //typeof(UnityEngine.UI.Selectable),
        //typeof(UnityEngine.UI.Slider),
        //typeof(UnityEngine.UI.Graphic),
        //typeof(UnityEngine.UI.MaskableGraphic),
        //typeof(UnityEngine.UI.Image),
        //typeof(UnityEngine.UI.Text),
        typeof(UnityEngine.Events.UnityEventBase),
        typeof(UnityEngine.Events.UnityEvent<>),
        typeof(UnityEngine.Events.UnityEvent),
        //typeof(UnityEngine.UI.Slider.SliderEvent),

        typeof(UnityEngine.Canvas),
//#endif
    };
    

    // some public class members can be used
    // some of them are only in editor mode
    // some are because of unknown reason
    //
    // they can't be distinguished by code, but can be known by checking unity docs
    public static bool IsDiscard(Type type, MemberInfo memberInfo)
    {
        string memberName = memberInfo.Name;

        if (typeof(Delegate).IsAssignableFrom(type)/* && (memberInfo is MethodInfo || memberInfo is PropertyInfo || memberInfo is FieldInfo)*/)
        {
            return true;
        }

        if (memberName == "networkView" && (type == typeof(GameObject) || typeof(Component).IsAssignableFrom(type)))
        {
            return true;
        }

        if ((type == typeof(Application) && memberName == "ExternalEval") ||
                        (type == typeof(Input) && memberName == "IsJoystickPreconfigured"))
        {
            return true;
        }
            
        //
        // Temporarily commented out
        // Uncomment them yourself!!
        //
        if ((type == typeof(Motion)) ||
            (type == typeof(AnimationClip) && memberInfo.DeclaringType == typeof(Motion)) ||
            (type == typeof(Application) && memberName == "ExternalEval") ||
            (type == typeof(Input) && memberName == "IsJoystickPreconfigured") ||
            (type == typeof(AnimatorOverrideController) && memberName == "PerformOverrideClipListCleanup") ||
            (type == typeof(Caching) && (memberName == "ResetNoBackupFlag" || memberName == "SetNoBackupFlag")) ||
            (type == typeof(Light) && (memberName == "areaSize")) ||
            (type == typeof(Security) && memberName == "GetChainOfTrustValue") ||
            (type == typeof(Texture2D) && memberName == "alphaIsTransparency") ||
            (type == typeof(WebCamTexture) && (memberName == "isReadable" || memberName == "MarkNonReadable")) ||
            (type == typeof(StreamReader) && (memberName == "CreateObjRef" || memberName == "GetLifetimeService" || memberName == "InitializeLifetimeService")) ||
            (type == typeof(StreamWriter) && (memberName == "CreateObjRef" || memberName == "GetLifetimeService" || memberName == "InitializeLifetimeService")) ||
            (type == typeof(UnityEngine.Font) && memberName == "textureRebuildCallback")
#if UNITY_4_6 || UNITY_4_7
             || (type == typeof(UnityEngine.EventSystems.PointerEventData) && memberName == "lastPress")
             || (type == typeof(UnityEngine.UI.InputField) && memberName == "onValidateInput") // property delegate
		    || (type == typeof(UnityEngine.UI.Graphic) && memberName == "OnRebuildRequested")
		    || (type == typeof(UnityEngine.UI.Text) && memberName == "OnRebuildRequested")
#endif
)
        {
            return true;
        }

#if UNITY_ANDROID || UNITY_IPHONE
        if (type == typeof(WWW) && (memberName == "movie"))
            return true;
#endif
        return false;
	}
	
	public static bool IsSupportByDotNet2SubSet(string functionName)
	{
		if (functionName == "Directory_CreateDirectory__String__DirectorySecurity" ||
		    functionName == "Directory_GetAccessControl__String__AccessControlSections" ||
		    functionName == "Directory_GetAccessControl__String" ||
		    functionName == "Directory_SetAccessControl__String__DirectorySecurity" ||
		    functionName == "DirectoryInfo_Create__DirectorySecurity" ||
		    functionName == "DirectoryInfo_CreateSubdirectory__String__DirectorySecurity" ||
		    functionName == "DirectoryInfo_GetAccessControl__AccessControlSections" ||
		    functionName == "DirectoryInfo_GetAccessControl" ||
		    functionName == "DirectoryInfo_SetAccessControl__DirectorySecurity" ||
		    functionName == "File_Create__String__Int32__FileOptions__FileSecurity" ||
		    functionName == "File_Create__String__Int32__FileOptions" ||
		    functionName == "File_GetAccessControl__String__AccessControlSections" ||
		    functionName == "File_GetAccessControl__String" ||
		    functionName == "File_SetAccessControl__String__FileSecurity")
		{
			return false;
		}
		return true;
	}

    public static bool NeedGenDefaultConstructor(Type type)
    {
        if (typeof(Delegate).IsAssignableFrom(type))
            return false;

        if (type.IsInterface)
            return false;

        // don't add default constructor
        // if it has non-public constructors
        // (also check parameter count is 0?)
        if (type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).Length != 0)
            return false;

        //foreach (var c in type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance))
        //{
        //    if (c.GetParameters().Length == 0)
        //        return false;
        //}

        if (type.IsClass && (type.IsAbstract || type.IsInterface))
            return false;

        if (type.IsClass)
        {
            return type.GetConstructors().Length == 0;
        }
        else
        {
            foreach (var c in type.GetConstructors())
            {
                if (c.GetParameters().Length == 0)
                    return false;
            }
            return true;
        }
    }

	public static bool CheckClassBindings(Type[] types)
	{
		Dictionary<Type, bool> clrLibrary = new Dictionary<Type, bool>();
		{
			//
			// these types are defined in clrlibrary.javascript
			//
			clrLibrary.Add(typeof(System.Object), true);
			clrLibrary.Add(typeof(System.Exception), true);
			clrLibrary.Add(typeof(System.SystemException), true);
			clrLibrary.Add(typeof(System.ValueType), true);
		}
		
		Dictionary<Type, bool> dict = new Dictionary<Type, bool>();
		var sb = new StringBuilder();
		bool ret = true;
		
		// can not export a type twice
		foreach (var type in types)
		{
			if (typeof(System.Delegate).IsAssignableFrom(type))
			{
				sb.AppendFormat("\"{0}\" Delegate can not be exported.\n",
				                JSNameMgr.GetTypeFullName(type));
				ret = false;
			}
			
			if (type.IsGenericType && !type.IsGenericTypeDefinition)
			{
				sb.AppendFormat(
					"\"{0}\" is not allowed. Try \"{1}\".\n",
					JSNameMgr.GetTypeFullName(type), JSNameMgr.GetTypeFullName(type.GetGenericTypeDefinition()));
				ret = false;
			}
			
			if (dict.ContainsKey(type))
			{
				sb.AppendFormat(
					"Operation fail. There are more than 1 \"{0}\" in JSBindingSettings.classes, please check.\n",
					JSNameMgr.GetTypeFullName(type));
				ret = false;
			}
			else
			{
				dict.Add(type, true);
			}
		}
		
		// Is BaseType exported?
		foreach (var typeb in dict)
		{
			Type type = typeb.Key;
			Type baseType = type.BaseType;
			if (baseType == null) { continue;  }
			if (baseType.IsGenericType) baseType = baseType.GetGenericTypeDefinition();
			// System.Object is already defined in SharpKit clrlibrary
			if (!clrLibrary.ContainsKey(baseType) && !dict.ContainsKey(baseType))
			{
				sb.AppendFormat("\"{0}\"\'s base type \"{1}\" must also be in JSBindingSettings.classes.\n",
				                JSNameMgr.GetTypeFullName(type),
				                JSNameMgr.GetTypeFullName(baseType));
				ret = false;
			}
			
			// 检查 interface 有没有配置		
			Type[] interfaces = type.GetInterfaces();
			for (int i = 0; i < interfaces.Length; i++)
			{
				Type ti = interfaces[i];
				
				string tiFullName = JSNameMgr.GetTypeFullName(ti);
				
				// 这个检查有点奇葩
				// 有些接口带 <>，这里直接忽略，不检查他
				if (!tiFullName.Contains("<") && tiFullName.Contains(">") && 
				    !clrLibrary.ContainsKey(ti) && !dict.ContainsKey(ti))
				{
					sb.AppendFormat("\"{0}\"\'s interface \"{1}\" must also be in JSBindingSettings.classes.\n",
                                    JSNameMgr.GetTypeFullName(type),
                                    JSNameMgr.GetTypeFullName(ti));
                    ret = false;
                }
            }
        }
        if (!ret)
        {
            Debug.LogError(sb);
        }
        return ret;
    }
}
