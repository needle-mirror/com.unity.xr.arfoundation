#ifndef XRAY_COMMON_INCLUDED
#define XRAY_COMMON_INCLUDED

static const half _FloorEdgeFadeScale = 10;
static const half _CeilingPeelbackScale = 5;
static const half _FadeThickness = 0.025;

half3 _RoomCenter = half3(0,0,0);
half _FloorHeight = 0.0;
half _CeilingHeight = 2.5;
half _RoomClipOffset = .5;
half _XRayScale = 1.0;

float GetCameraPositionWSHeight()
{
#if defined(XRAY_ENABLED)
    #if defined(INCLUDE_RENDER_PIPELINES_UNIVERSAL) && !defined(LEGACY_IN_SRP)
        return GetCameraPositionWS().y;
    #else
        return _WorldSpaceCameraPos.y;
    #endif
#else
    return 1.0f;
#endif
}

float2 GetWorldSpaceXZPlaneIntersection()
{
#if defined(USING_STEREO_MATRICES) || defined(LEGACY_IN_SRP)
    return float2(unity_CameraToWorld._m02, unity_CameraToWorld._m22);
#else
    #if defined(INCLUDE_RENDER_PIPELINES_UNIVERSAL) && defined(XRAY_FLIP_DEPTH)
        return float2(-unity_CameraToWorld._m02, -unity_CameraToWorld._m22);
    #else
        return float2(unity_CameraToWorld._m02, unity_CameraToWorld._m22);
    #endif
#endif
}

half getXRayFade(half3 coords)
{
    coords = (coords - _RoomCenter) / _XRayScale;
    half cameraY = (GetCameraPositionWSHeight() - _RoomCenter.y) / _XRayScale;

    // Get where the clip plane should cut off at, further adjusted by camera height
    half intersectionPlaneThickness = _RoomClipOffset - _CeilingPeelbackScale * max(cameraY - _CeilingHeight, 0) * saturate(coords.y - _CeilingHeight);

    // Get the current pixel's distance from the clip plane
    half2 intersectionLine = -normalize(GetWorldSpaceXZPlaneIntersection());
    half2x2 intersectionPlaneSpace = half2x2(half2(intersectionLine.x, intersectionLine.y), half2(-intersectionLine.y, intersectionLine.x));
    half intersectionPlaneDistance = mul(intersectionPlaneSpace, coords.xz).x;

    half inFrontOfClipPlaneIntersectionPlane = saturate(coords.y - _FloorHeight) * saturate(intersectionPlaneDistance - intersectionPlaneThickness);
    half cameraAndPixelBelowFloorPlane = saturate(_FloorHeight - GetCameraPositionWSHeight()) * saturate(_FloorHeight - coords.y);

    // Stop drawing if we are outside any of the drawing regions
    clip(-1 * (inFrontOfClipPlaneIntersectionPlane + cameraAndPixelBelowFloorPlane)
        #ifdef SHADOWS_DEPTH            // Hack to detect we are in the shadow-caster phase (but not shadow receiver) and properly prevent the light from clipping out
            + UNITY_MATRIX_P[3][3] * 4
        #endif
     );

    return  1 - saturate((intersectionPlaneDistance + _FadeThickness - intersectionPlaneThickness) / _FadeThickness) * saturate((coords.y - _FloorHeight) * _FloorEdgeFadeScale);
}

half getXRayEdgeFade(half3 coords)
{
    coords = (coords - _RoomCenter) / _XRayScale;
    half cameraY = (GetCameraPositionWSHeight() - _RoomCenter.y) / _XRayScale;

    // Get where the clip plane should cut off at, further adjusted by camera height
    half intersectionPlaneThickness = _RoomClipOffset - _CeilingPeelbackScale * max(cameraY - _CeilingHeight, 0) * saturate(coords.y - _CeilingHeight);

    // Get the current pixel's distance from the clip plane
    half2 intersectionLine = -normalize(GetWorldSpaceXZPlaneIntersection());
    half2x2 intersectionPlaneSpace = half2x2(half2(intersectionLine.x, intersectionLine.y), half2(-intersectionLine.y, intersectionLine.x));
    half intersectionPlaneDistance = mul(intersectionPlaneSpace, coords.xz).x;

    half betweenFadePlanes = saturate(coords.y - _FloorHeight) * (saturate(intersectionPlaneDistance - _FadeThickness - intersectionPlaneThickness) + saturate(-intersectionPlaneDistance + intersectionPlaneThickness));
    half pixelBelowFloorPlane = saturate(_FloorHeight - coords.y);

    // Stop drawing if we are outside any of the drawing regions
    clip(-1 * (betweenFadePlanes + pixelBelowFloorPlane));

    return 1 - saturate((intersectionPlaneDistance - intersectionPlaneThickness) / _FadeThickness)*saturate((coords.y - _FloorHeight) * _FloorEdgeFadeScale);
}

#endif
