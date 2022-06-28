# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

$startTime = Get-Date

# If you have a commit that contains only the meta file changes, you can write them into this format via:
# git show [commit hash] | Select-String '(^[+-][^+-])|([+] b/)' | ForEach-Object {
#     $count++
#     $output = $_.ToString().Substring(1).Trim()
#     if ($count % 3 -eq 0) {
#         Add-Content -Path guids.txt -Value " = `'$output`' # $fileName"
#     }
#     elseif ($count % 3 -eq 1) {
#         $fileName = $output.Substring(5).Trim()
#     }
#     else {
#         Add-Content -Path guids.txt -Value "`'$output`'" -NoNewLine
#     }
# }

$lookupTable = @{
    'guid: 248664dc0e60dca4fb35d70cd0c5695b' = 'guid: 511768f84882a6447ab13fd3a09c482a' # Core/Utilities/HandBounds.cs.meta
    'guid: d199a2ca60343bb49ad9a41ddb45a083' = 'guid: eda2633c745b44f478a079be01cf388e' # Diagnostics/Visualizers/Shaders/InstancedColored.shader.meta
    'guid: 36e555a3b848ed844a6774ec59b878d2' = 'guid: 94293738b8642e3488ebda142b01390c' # ExtendedAssets/Audio/PianoNotes/A.wav.meta
    'guid: c376d65467e4f844ba94a19350918427' = 'guid: 6a443adb97d99dc4abc87af23f807484' # ExtendedAssets/Audio/PianoNotes/B.wav.meta
    'guid: cb811ff0fcdbb2a40a8398f721cc28a1' = 'guid: 6f8d07139b424644c893be10a8f5c186' # ExtendedAssets/Audio/PianoNotes/BFlat.wav.meta
    'guid: c9292cc9ccb6e104e8448c8aec5603ff' = 'guid: 752f42bf6dfcb57438f951a7ea68cc60' # ExtendedAssets/Audio/PianoNotes/C.wav.meta
    'guid: e4154763f07adb7418e874d20d0bfc37' = 'guid: 4508b3feb2b8d2d41a1d5db4bbf1a7ab' # ExtendedAssets/Audio/PianoNotes/CSharp.wav.meta
    'guid: ebe03136b6c6de34a9a4cd1b32c1124d' = 'guid: eee0c8b3256cb9a41b353e0d7f56b2d8' # ExtendedAssets/Audio/PianoNotes/C_Octave.wav.meta
    'guid: dbaf577a8176a8a49aeee479ce6fba46' = 'guid: b2d1103908324c34389d82e719b448e2' # ExtendedAssets/Audio/PianoNotes/D.wav.meta
    'guid: 6bb977d2eec33d34e9d4683d95a325da' = 'guid: 163c64a21f29e7141a99be24c2a6515e' # ExtendedAssets/Audio/PianoNotes/E.wav.meta
    'guid: 5ad4c6b5880452d4a813c8714b5d4338' = 'guid: 12d663c06597b134281f7115e98686df' # ExtendedAssets/Audio/PianoNotes/EFlat.wav.meta
    'guid: 1e3cd518fc252894f8dcede422f1792f' = 'guid: b31b2e6932d89e24f8b486cc4da10655' # ExtendedAssets/Audio/PianoNotes/F.wav.meta
    'guid: 0008aa109f6a9f34b9658d73a0c38e74' = 'guid: 138cb566b4e35ba40a6d1cbe551db941' # ExtendedAssets/Audio/PianoNotes/FSharp.wav.meta
    'guid: 577b302235decc047a09254a91858236' = 'guid: 891f1aa637d913e4dbb10c3c403e17d0' # ExtendedAssets/Audio/PianoNotes/G.wav.meta
    'guid: b8c73329dfc0a344a8461642b241c30d' = 'guid: 26a44f0a23435f84487034bd6e50049b' # ExtendedAssets/Audio/PianoNotes/GSharp.wav.meta
    'guid: 32aad78585988cc43838b0d2bd36aff3' = 'guid: 6e7447ead01a61e44844582900e6955e' # ExtendedAssets/FontSDF/DepthWriteOff/segoeui SDF_zwrite_off.asset.meta
    'guid: f137eba12ee10834cb19632437cfdb2e' = 'guid: e707a6a2ab95bc445aee7b61d2355253' # ExtendedAssets/FontSDF/DepthWriteOff/segoeuib SDF_zwrite_off.asset.meta
    'guid: cf40b52fb3478de4ea4adf277e3b75ef' = 'guid: e929782b75c1ba54c99f8f2235a88798' # ExtendedAssets/FontSDF/DepthWriteOff/segoeuil SDF_zwrite_off.asset.meta
    'guid: d94d0d64ec3545b408d5621e7d27cf96' = 'guid: 0f909757e0b32ca48a7127b2eac34321' # ExtendedAssets/FontSDF/DepthWriteOff/segoeuisl SDF_zwrite_off.asset.meta
    'guid: 8f25585131267564cab5bbdafd36a94f' = 'guid: 42116971a9b78104f9debc8841d587be' # ExtendedAssets/FontSDF/DepthWriteOff/seguibl SDF_zwrite_off.asset.meta
    'guid: ad601d838b0fb2744937cd5dad890e81' = 'guid: af3b1a4eb2d6b724985bbe8627b0c4c1' # ExtendedAssets/FontSDF/DepthWriteOff/seguisb SDF_zwrite_off.asset.meta
    'guid: afc8299d5d5bbd440a0616c8ecbc7217' = 'guid: 77f215f3ce99bbc42a3bea271f6915ee' # ExtendedAssets/FontSDF/DepthWriteOn/segoeui SDF.asset.meta
    'guid: 708ca4cbd1d60c64184b76fa75044402' = 'guid: 6adca8b32bf7c6346b94766fcb044b9f' # ExtendedAssets/FontSDF/DepthWriteOn/segoeuib SDF.asset.meta
    'guid: 8349af0188f70214ba25245a3c05ddeb' = 'guid: ef34674acfe6d3a428b8ed9628851460' # ExtendedAssets/FontSDF/DepthWriteOn/segoeuil SDF.asset.meta
    'guid: c334791383d1d8042b3a69a77b6b3283' = 'guid: 849495abf8312a24189930b1ebd4ad9a' # ExtendedAssets/FontSDF/DepthWriteOn/segoeuisl SDF.asset.meta
    'guid: 1e1a3bf70c1dc22478ff4fc9837e570e' = 'guid: 6949a1ca4bfe98e4092d1eede41189a7' # ExtendedAssets/FontSDF/DepthWriteOn/seguibl SDF.asset.meta
    'guid: 6a84f857bec7e7345843ae29404c57ce' = 'guid: 831100f41db48954f86d25ee28f9ce12' # ExtendedAssets/FontSDF/DepthWriteOn/seguisb SDF.asset.meta
    'guid: fc1d56dcfc4950949a5b56a1399a1822' = 'guid: 688dce1dbb1f15842b9bc2f79fc81c84' # ExtendedAssets/Materials/MRTK_GrabPointerGrabPoint.mat.meta
    'guid: e74b90b0fec1bef4fa500cebdb12d821' = 'guid: 8d7426afbead35b4caf90abb809ed21b' # ExtendedAssets/Materials/MRTK_GrabPointerTetherLine.mat.meta
    'guid: 862e98d97747dd24291ef5e8e2f13237' = 'guid: 5698508be10ccb54da6e1a239e211948' # ExtendedAssets/Models/CoffeeCup 1.fbm.meta
    'guid: 7c3aaa73ed84f5b4fb1c16afcb7bf70f' = 'guid: cbb5bc89096deea4bb1188129224afe1' # ExtendedAssets/Models/CoffeeCup 1.fbm/5.png.meta
    'guid: 061e0aef150f0d94d86e3767ed08be90' = 'guid: c5ee8ca13c5831545a95761faae8207f' # ExtendedAssets/Models/CoffeeCup 1.fbm/7.png.meta
    'guid: e963263242b6cbb4bbbf279f0c0e7789' = 'guid: 6ceef383a3dfd0a4e9c38b8d793170e4' # ExtendedAssets/Models/CoffeeCup.fbx.meta
    'guid: ea710f22bf4720243847407cdb0b355b' = 'guid: b61ab5456e822e04ea60003311668940' # ExtendedAssets/Models/EarthCore.fbm.meta
    'guid: 3a203b931b1b1ef4f8fedcf179dac304' = 'guid: 11d778e0f745dd44dbed92d9c0a85b96' # ExtendedAssets/Models/EarthCore.fbm/5.png.meta
    'guid: e502d6f9910b62944ac502b8671c9695' = 'guid: 7f54a1a9c3ec9cb428c77e0fe863d58b' # ExtendedAssets/Models/EarthCore.fbm/7.png.meta
    'guid: a817fa37f27ca3a4fa16203c8043be4a' = 'guid: 638d4274791c60048aeaeb3e42b2d40d' # ExtendedAssets/Models/EarthCore.fbm/MaterialEarth.mat.meta
    'guid: c72691f7b364d264abdb41ef2f01c706' = 'guid: 0a30b3edc4f8ea24da9177aeae1b1e34' # ExtendedAssets/Models/EarthCore.fbx.meta
    'guid: 28de8cd8a6b1f454885cb901c876bb45' = 'guid: 1b72c1e6302ca7542afd89257b6367aa' # ExtendedAssets/Models/HumanHeart.fbx.meta
    'guid: 1b1d8c45e1e584240b13d1fc6fbb99a6' = 'guid: f9a8072f5d9b92347a999f10f01b8012' # ExtendedAssets/Models/Lander.fbx.meta
    'guid: 9b8d622e06b5ddc47bfd77b86d50527c' = 'guid: 60260aaff9ed8764d9aad59253d848f7' # ExtendedAssets/Models/LunarModule.fbx.meta
    'guid: 902b4366116e1184aaee744612f65d77' = 'guid: 806a81db06918064ba8a265deab8e558' # ExtendedAssets/Models/MarsCuriosityRover.FBX.meta
    'guid: 648e0abb932ec334996e48c9d6b69a9f' = 'guid: b3b4bdd3f306bde4593656bdd9995684' # ExtendedAssets/Models/MarsCuriosityRover.fbm.meta
    'guid: 573b894ed1c2fc44ea2541ee5b8a57ad' = 'guid: 2efcfe3b19f827b42b441cf7997a43d3' # ExtendedAssets/Models/MarsCuriosityRover.fbm/10.png.meta
    'guid: 7beab324a3ae347418d5bd86646a4c13' = 'guid: 3386577c07b64fc4b91ca42fc1169276' # ExtendedAssets/Models/MarsCuriosityRover.fbm/5.png.meta
    'guid: 3a3b17c9ed7c80c459e267bea86883a2' = 'guid: ae9344204af18344d93ff8bcc432f3f6' # ExtendedAssets/Models/MarsCuriosityRover.fbm/6.png.meta
    'guid: 63df0e3a2395a334baf386aeb665335d' = 'guid: 554a14b58b69adb489f83175408dc134' # ExtendedAssets/Models/MarsCuriosityRover.fbm/7.png.meta
    'guid: 1555559c1bef31644a62e9c2aa236c2b' = 'guid: 4ac4fedf69b50e24c9bf67e7f6cddc8f' # ExtendedAssets/Models/MarsCuriosityRover.fbm/8.png.meta
    'guid: 9aca6013df8837448bc14e4b117cb609' = 'guid: c78ef35bd9ed8894fb30e1d6ac8c78af' # ExtendedAssets/Models/MarsCuriosityRover.fbm/9.png.meta
    'guid: 699f5a92df552f645aaea84e28ab7693' = 'guid: fe95f0d8b06f9ab43b02cf635280051b' # ExtendedAssets/Models/Materials.meta
    'guid: 1c8a9561fb418d140bd508ccf24fe2a5' = 'guid: 138e55b367631c94bbf69e76b0aadd20' # ExtendedAssets/Models/Materials/10.mat.meta
    'guid: b62873551dfeef943be6324c304cd0ab' = 'guid: 2877a47ddf5b5d04d8c3cac2862ddf25' # ExtendedAssets/Models/Materials/5.mat.meta
    'guid: bfc246c5506b90144bf15735519c217c' = 'guid: 9db46b2aafe195d4596e1eb7e29e0a0e' # ExtendedAssets/Models/Materials/6.mat.meta
    'guid: 0393ac2b971eddd4785463e9f09c243c' = 'guid: 15bea05ae1c74194fa218e0d4a8a1b25' # ExtendedAssets/Models/Materials/7.mat.meta
    'guid: bc50df427b3cb4b48931176cb316d8ab' = 'guid: c3e54393dc85829478f6be234486226f' # ExtendedAssets/Models/Materials/8.mat.meta
    'guid: 0b03c720042dbe146afee09a803c4c9b' = 'guid: f51d820ea4b05da4c85fdd0f3a4aabc3' # ExtendedAssets/Models/Materials/9.mat.meta
    'guid: 27cd524eafb15f74bac1f22817647b78' = 'guid: 9e40691c4d353d54fad64be5bf93e3a8' # ExtendedAssets/Models/Materials/Material_56.mat.meta
    'guid: 8f1d5bfb62fbcb24290575c02eb7fe3c' = 'guid: 214afd77a2dddcf44b1452ced655e544' # ExtendedAssets/Models/Materials/Material_60.mat.meta
    'guid: 40bb9772594a93140a43a9a4f5cf9356' = 'guid: 13c4664da4c66114c8bd6616ab5d6fe4' # ExtendedAssets/Models/Model_Octa.fbx.meta
    'guid: f9b1acc0404b53f45bffb480fefa205a' = 'guid: dd0cc440d56588046a9c0df7183f8de5' # ExtendedAssets/Models/Model_Platonic.fbx.meta
    'guid: 728972833a3739d4fa5d234f7c91b4b2' = 'guid: 2e4e683eccb3efe49ba775ada36cbc8f' # ExtendedAssets/Models/Model_PushButton.fbx.meta
    'guid: ffbe4bef620a0a542bb77fe2f765c905' = 'guid: 8f6fb5f907837b645a884d67d7b70393' # ExtendedAssets/Models/Piano.prefab.meta
    'guid: 6c9893c878a0faf439830c1d014e9e4e' = 'guid: 3092be637eba36446a14b2e6b486f72b' # ExtendedAssets/Models/PianoBlackKey.prefab.meta
    'guid: f47f1e79863e27c46904308902d30551' = 'guid: e8ad096e4411c7146acb447352e7b520' # ExtendedAssets/Models/PianoKey.prefab.meta
    'guid: bb0ccc5c35ed851498eb51db45d1a738' = 'guid: aafe2432fe2b1a745b6b324e7fc3d415' # ExtendedAssets/Models/PianoKeys.fbx.meta
    'guid: a37785fa4a4129246a58f79132a1b6c6' = 'guid: 5e331400f64621b439dbed5d1a172aac' # ExtendedAssets/Models/PianoModel.fbx.meta
    'guid: 2e0a998ea0d9be24fa2ef41c5e813f2e' = 'guid: a03ba5967587e36469c6da199f3ec8d3' # ExtendedAssets/Models/Prototyping.meta
    'guid: dc6c0266b7f350f40b24b8ddfae317b4' = 'guid: 6988e14e52783e347b09ecad4bed776f' # ExtendedAssets/Models/Prototyping/Chevron.fbx.meta
    'guid: 841b5755ac02dbc439bd347f414de999' = 'guid: cf146601d58ac864d88e2c348d719c73' # ExtendedAssets/Models/Prototyping/Torus.fbx.meta
    'guid: 1104611d051d4e42a68391888b61b32e' = 'guid: ae5a20763aec11648adc7193e64d3094' # ExtendedAssets/Models/ShaderBall.FBX.meta
    'guid: b32f0db548a77f14aa0f44dd3e1829bf' = 'guid: d86749b59e7e1d54b96ba3acad2fc18c' # ExtendedAssets/Models/Tree.fbm.meta
    'guid: abc0278c36965d643b5d21021630ee68' = 'guid: c1d1fc1d0afdabd43a6d628bfc66e26c' # ExtendedAssets/Models/Tree.fbm/5.png.meta
    'guid: 08640538943897847b0d7d46ae188811' = 'guid: 6e0f8b8da5f22fa46bcb5c36c0fb8b19' # ExtendedAssets/Models/Tree.fbm/6.png.meta
    'guid: ded4159fdff3e874580aa6c6cf2e0259' = 'guid: 227dbcfeb628d8645928eeb4c44019d1' # ExtendedAssets/Models/Tree.fbm/MaterialTree.mat.meta
    'guid: d45e66785744f1d429a96d32f3f705d4' = 'guid: f967b4d6f21546b46a9254c5689467e2' # ExtendedAssets/Models/Tree.fbx.meta
    'guid: 23d0d428664728f48bc426a2842ced95' = 'guid: 4f0cf994edc095e40a08441132e7f905' # ExtendedAssets/Models/balloon.fbx.meta
    'guid: ac4decb25aa59fd47acecae2afb00d9e' = 'guid: 577009e68d3b61243b224142cc415a26' # ExtendedAssets/Textures/Bricks_normal.png.meta
    'guid: 659a33cb4a7138343ae519b13efd2375' = 'guid: 3a18a174c9cdea843b49195de89efc5e' # ExtendedAssets/Textures/CoffeeFoam.png.meta
    'guid: fe6c9717b3426224e800bc2ccec9169e' = 'guid: 1393ebb2c8d46f44fa996199695d79d4' # ExtendedAssets/Textures/Fabric_normal.png.meta
    'guid: bad97593e042d8545aadb269c3f4bc01' = 'guid: 949533dcd9b0bba46a43427bb889d111' # ExtendedAssets/Textures/HumanHeart_Albdeo.png.meta
    'guid: 11056bb52f91cdc43ba4fba9accfb226' = 'guid: 00d7cdf7a47872a4194c915b53ed519a' # ExtendedAssets/Textures/Lander_albedo.png.meta
    'guid: bf08fbb83f27bc2408110dba26a36a7f' = 'guid: b43c63e30e3dab244bcdbc1bd5cf7ce8' # ExtendedAssets/Textures/Lander_channel.png.meta
    'guid: 114a88419a74a534197624fb7bfe17da' = 'guid: 4ed091b66a7402043b459a25805ab938' # ExtendedAssets/Textures/Lander_normal.png.meta
    'guid: 7b551659cf4349242ba72d82b4f9cdc7' = 'guid: c0632a8c99be41848b2d0926a62b671c' # ExtendedAssets/Textures/Panel_albedo.png.meta
    'guid: bb3fa60902dd45d47af0ab86831f8c2c' = 'guid: 91ca67a24f12ed643879d7f63f404454' # ExtendedAssets/Textures/PianoKeys_DIF.png.meta
    'guid: 6e452eb16719c234cb5b06d5f0a00834' = 'guid: a784aea9168fca74db266f4f2eb1fa9f' # ExtendedAssets/Textures/PianoKeys_NM.png.meta
    'guid: b43e6b5cc1034971b8dcbb3ed95217fe' = 'guid: e253cc5e92619774481b290e464c4dd2' # ExtendedAssets/Textures/RustedIron_albedo.png.meta
    'guid: 6392e9fd2e0d6b44d932c8bb2c062fdd' = 'guid: bcac5bff682e77a4d8628fdd2ca2e7ca' # ExtendedAssets/Textures/RustedIron_channel.png.meta
    'guid: 07e377645ecb440bb56a3d97bf914134' = 'guid: c4f37648ac6d26644a7d69d69fdba5d4' # ExtendedAssets/Textures/RustedIron_normal.png.meta
    'guid: c002272d1f63bd448acdb2375eb9a94d' = 'guid: c3b87ec93676701418ddc1b550238e79' # ExtendedAssets/Textures/ShaderBallNoise_albedo.png.meta
    'guid: 3f4d23ee9ceed344593bbe0526ab1d46' = 'guid: 063c9d1299491ba4495db36f06f88d40' # ExtendedAssets/Textures/ShaderBall_albedo.png.meta
    'guid: 32c57306df745a147a2e02f8827a8cdc' = 'guid: 15b390e1957e43a49bb8fffeb0380a6a' # ExtendedAssets/Textures/ShaderBall_ao.png.meta
    'guid: e2c3b29c57644d009d3373c52e72b7bf' = 'guid: f520c8b3133d3b044a1fa6fe6af6d52b' # Input/Interactors/InteractorVisuals/Lines/DataProviders/BaseMixedRealityLineDataProvider.cs.meta
    'guid: 04b1ba3412a235c4d8c3eb2a18528b67' = 'guid: 8b3e34902948c18428b36f8c4ed4e153' # Input/Interactors/InteractorVisuals/Lines/DataProviders/BezierDataProvider.cs.meta
    'guid: a8d1152962ef444c8aa086fe80143d71' = 'guid: 53fee3a30e2baa04197d935275a4a413' # SpatialManipulation/Solvers/ConstantViewSize.cs.meta
    'guid: c45719620cce37b49bae491053c48bed' = 'guid: 419439fafd8ae7f4abdae12c1eec19f1' # SpatialManipulation/Solvers/DirectionalIndicator.cs.meta
    'guid: 4479a0bd44d822241a4c5f4890c481c0' = 'guid: 07606d041cd57684c85e7122ad8e3897' # SpatialManipulation/Solvers/Follow.cs.meta
    'guid: e6dd43f06873cf8408515860553aefb0' = 'guid: edc4e167468d3784ca3b156d73bfd764' # SpatialManipulation/Solvers/HandConstraint.cs.meta
    'guid: 956af6ee031a2eb47b9c0ab5dcd50fd7' = 'guid: ebb10c5ec282c8044a11e2f0f9c65a80' # SpatialManipulation/Solvers/HandConstraintPalmUp.cs.meta
    'guid: de9b118fbc66f114cac64e9b05576a1b' = 'guid: 61d9e86fbd30e6a4ca9873d2605ba4ed' # SpatialManipulation/Solvers/InBetween.cs.meta
    'guid: 603efe86530a40098e384687f87daf08' = 'guid: 854553c3b058c1f4b9ebba2b58c10629' # SpatialManipulation/Solvers/Momentum.cs.meta
    'guid: 14c3d8a4208d4b649529822e217623d4' = 'guid: 5a8e5e857dcf3a84cbd29548d8c79aa5' # SpatialManipulation/Solvers/Orbital.cs.meta
    'guid: 97026b56b4e804b468b8d53af13e3c3b' = 'guid: 1cd106bf943eaa1439d774d7143f5f7d' # SpatialManipulation/Solvers/Overlap.cs.meta
    'guid: 4684083f6dff4a1d8a790bccc354fcf4' = 'guid: 4c8a10bdd6850674590a7d931a87adb3' # SpatialManipulation/Solvers/RadialView.cs.meta
    'guid: 3e9078183f524b1baa8197daf640b60f' = 'guid: 24445032f6e7f2449a22209821c86607' # SpatialManipulation/Solvers/SurfaceMagnetism.cs.meta
    'guid: e1e1c3953429c5e4fb804cd997540db9' = 'guid: ef52421bb8639ef4a80a74143e7afcfe' # SpatialManipulation/Solvers/TapToPlace.cs.meta
    'guid: 93ce3152880942b29522d76da2187beb' = 'guid: 537c5299a4b9dfd4d8507f885613b001' # SpatialManipulation/Utilities/Solver.cs.meta
    'guid: b55691ad5b034fe6966763a6e23818d2' = 'guid: 3986155c7a728454f8bbbabd2e274601' # SpatialManipulation/Utilities/SolverHandler.cs.meta
    'guid: 5d591e6dd5d05804f8c9bbbf00106499' = 'guid: 91849fe87eeef91428a2370754fbaf10' # StandardAssets/Utilities/CameraEventRouter.cs.meta
    'guid: 45f22dce2073a684596a16ab838956d3' = 'guid: c53e2b0613597e849870c4a691d25e0f' # StandardAssets/Utilities/MaterialInstance.cs.meta
    'guid: 494f23d66f4f1fe40ab0ee05fe75e766' = 'guid: 6cc71cf53860e9a4b8cc5b2aed46bb76' # UnityProjects/MRTKDevTemplate/Assets/Scripts/ColorChanger.cs.meta
}

$guids = $lookupTable.Keys -join '|'
$count = 0
$checkedCount = 0

Get-ChildItem . -File -Recurse -Include *.prefab, *.unity, *.asset, *.mat, *.meta | Where-Object { $_.FullName -NotLike "*Library*" } | ForEach-Object {
    $checkedCount++
    $file = Get-Content $_.FullName
    $containsGuid = $file | ForEach-Object { $_ -match ($guids) }
    if ($containsGuid -contains $true) {
        ($file) | ForEach-Object {
            $line = $_
            $lookupTable.GetEnumerator() | ForEach-Object {
                $line = $line -replace $_.Key , $_.Value
            }
            $line } | Set-Content $_.FullName
        Write-Host -NoNewline "!"
        $count++
    }
    else {
        Write-Host -NoNewline "."
    }
}

$endTime = Get-Date
$totalTime = "{0:mm:ss}" -f ([DateTime]($endTime - $startTime).Ticks)
Write-Host "`nUpdated $count files (out of $checkedCount checked) in $totalTime"
Pause
