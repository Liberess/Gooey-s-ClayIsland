
#define rot(x, k) (((x) << (k)) | ((x) >> (32 - (k))))

#define __float_as_uint(a) (uint(a))
#define make_float2(a, b) (float2(a, b))
#define make_float3(a, b, c) (float3(a, b, c))
#define make_float4(a, b, c, d) (float4(a, b, c, d))
#define fabsf(x) abs(((float)x))
#define powf(x, y) pow(((float)x), ((float)y))

#define mix(a, b, c) float3(a, b, c)

#define mix(a, b, c) { a -= c; }

#define mix(a, b, c) \
    { \
      a -= c; \
      a ^= rot(c, 4); \
      c += b; \
      b -= a; \
      b ^= rot(a, 6); \
      a += c; \
      c -= b; \
      c ^= rot(b, 8); \
      b += a; \
      a -= c; \
      a ^= rot(c, 16); \
      c += b; \
      b -= a; \
      b ^= rot(a, 19); \
      a += c; \
      c -= b; \
      c ^= rot(b, 4); \
      b += a; \
    }

#define final(a, b, c) \
  { \
    c ^= b; \
    c -= rot(b, 14); \
    a ^= c; \
    a -= rot(c, 11); \
    b ^= a; \
    b -= rot(a, 25); \
    c ^= b; \
    c -= rot(b, 16); \
    a ^= c; \
    a -= rot(c, 4); \
    b ^= a; \
    b -= rot(a, 14); \
    c ^= b; \
    c -= rot(b, 24); \
  }

inline uint hash_uint(uint kx)
{
	uint a, b, c;
	a = b = c = 0xdeadbeef + (1 << 2) + 13;

	a += kx;
	final(a, b, c);

	return c;
}

inline uint hash_uint2(uint kx, uint ky)
{
	uint a, b, c;
	a = b = c = 0xdeadbeef + (2 << 2) + 13;

	b += ky;
	a += kx;
	final(a, b, c);

	return c;
}

inline uint hash_uint3(uint kx, uint ky, uint kz)
{
	uint a, b, c;
	a = b = c = 0xdeadbeef + (3 << 2) + 13;

	c += kz;
	b += ky;
	a += kx;
	final(a, b, c);

	return c;
}

inline uint hash_uint4(uint kx, uint ky, uint kz, uint kw)
{
	uint a, b, c;
	a = b = c = 0xdeadbeef + (4 << 2) + 13;

	a += kx;
	b += ky;
	c += kz;
	mix(a, b, c);

	a += kw;
	final(a, b, c);

	return c;
}

/* Hashing uint or uint[234] into a float in the range [0, 1]. */

inline float hash_uint_to_float(uint kx)
{
	return (float)hash_uint(kx) / (float)0xFFFFFFFFu;
}

inline float hash_uint2_to_float(uint kx, uint ky)
{
	return (float)hash_uint2(kx, ky) / (float)0xFFFFFFFFu;
}

inline float hash_uint3_to_float(uint kx, uint ky, uint kz)
{
	return (float)hash_uint3(kx, ky, kz) / (float)0xFFFFFFFFu;
}

inline float hash_uint4_to_float(uint kx, uint ky, uint kz, uint kw)
{
	return (float)hash_uint4(kx, ky, kz, kw) / (float)0xFFFFFFFFu;
}

/* Hashing float or float[234] into a float in the range [0, 1]. */

inline float hash_float_to_float(float k)
{
	return hash_uint_to_float(__float_as_uint(k));
}

inline float hash_float2_to_float(float2 k)
{
	return hash_uint2_to_float(__float_as_uint(k.x), __float_as_uint(k.y));
}

inline float hash_float3_to_float(float3 k)
{
	return hash_uint3_to_float(__float_as_uint(k.x), __float_as_uint(k.y), __float_as_uint(k.z));
}

inline float hash_float4_to_float(float4 k)
{
	return hash_uint4_to_float(
		__float_as_uint(k.x), __float_as_uint(k.y), __float_as_uint(k.z), __float_as_uint(k.w));
}

/* Hashing float[234] into float[234] of components in the range [0, 1]. */

inline float2 hash_float2_to_float2(float2 k)
{
	return make_float2(hash_float2_to_float(k), hash_float3_to_float(make_float3(k.x, k.y, 1.0)));
}

inline float3 hash_float3_to_float3(float3 k)
{
	return make_float3(hash_float3_to_float(k),
		hash_float4_to_float(make_float4(k.x, k.y, k.z, 1.0)),
		hash_float4_to_float(make_float4(k.x, k.y, k.z, 2.0)));
}

inline float4 hash_float4_to_float4(float4 k)
{
	return make_float4(hash_float4_to_float(k),
		hash_float4_to_float(make_float4(k.w, k.x, k.y, k.z)),
		hash_float4_to_float(make_float4(k.z, k.w, k.x, k.y)),
		hash_float4_to_float(make_float4(k.y, k.z, k.w, k.x)));
}

/* Hashing float or float[234] into float3 of components in range [0, 1]. */

inline float3 hash_float_to_float3(float k)
{
	return make_float3(hash_float_to_float(k),
		hash_float2_to_float(make_float2(k, 1.0)),
		hash_float2_to_float(make_float2(k, 2.0)));
}

inline float3 hash_float2_to_float3(float2 k)
{
	return make_float3(hash_float2_to_float(k),
		hash_float3_to_float(make_float3(k.x, k.y, 1.0)),
		hash_float3_to_float(make_float3(k.x, k.y, 2.0)));
}

inline float3 hash_float4_to_float3(float4 k)
{
	return make_float3(hash_float4_to_float(k),
		hash_float4_to_float(make_float4(k.z, k.x, k.w, k.y)),
		hash_float4_to_float(make_float4(k.w, k.z, k.y, k.x)));
}

/* SSE Versions Of Jenkins Lookup3 Hash Functions */

inline float voronoi_distance_1d(float a,
	float b,
	int metric,
	float exponent)
{
	return abs(b - a);
}

inline float2 unity_voronoi_noise_randomVector(float2 UV, float offset)
{
	float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
	UV = frac(sin(mul(UV, m)) * 46839.32);
	return float2(sin(UV.y * +offset) * 0.5 + 0.5, cos(UV.x * offset) * 0.5 + 0.5);
}

#define NODE_VORONOI_EUCLIDEAN 0
#define NODE_VORONOI_MANHATTAN 1
#define NODE_VORONOI_CHEBYCHEV 2
#define NODE_VORONOI_MINKOWSKI 3

inline float voronoi_distance_3d(float3 a,
	float3 b,
	int metric,
	float exponent)
{
	if (metric == NODE_VORONOI_EUCLIDEAN) {
		return distance(a, b);
	}
	else if (metric == NODE_VORONOI_MANHATTAN) {
		return fabsf(a.x - b.x) + fabsf(a.y - b.y) + fabsf(a.z - b.z);
	}
	else if (metric == NODE_VORONOI_CHEBYCHEV) {
		return max(fabsf(a.x - b.x), max(fabsf(a.y - b.y), fabsf(a.z - b.z)));
	}
	else if (metric == NODE_VORONOI_MINKOWSKI) {
		return powf(powf(fabsf(a.x - b.x), exponent) + powf(fabsf(a.y - b.y), exponent) +
			powf(fabsf(a.z - b.z), exponent),
			1.0f / exponent);
	}
	else {
		return 0.0f;
	}
}

void Unity_Voronoi_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
{
	float2 g = floor(UV * CellDensity);
	float2 f = frac(UV * CellDensity);
	float t = 8.0;
	float3 res = float3(8.0, 0.0, 0.0);

	for (int y = -1; y <= 1; y++)
	{
		for (int x = -1; x <= 1; x++)
		{
			float2 lattice = float2(x, y);
			float2 offset = unity_voronoi_noise_randomVector(lattice + g, AngleOffset);
			float d = distance(lattice + offset, f);
			if (d < res.x)
			{
				res = float3(d, offset.x, offset.y);
				Out = res.x;
				Cells = res.y;
			}
		}
	}
}

void Blender_Voronoi1d_float(float2 w, float exponent, float randomness, int metric, out float3 outColor, out float outDistance, out float outW)
{
	  float cellPosition = floor(w); 
	  float localPosition = w - cellPosition;
	
	  float minDistance = 8.0f;
	  float targetOffset = 0.0f;
	  float targetPosition = 0.0f; 
	  for (int i = -1; i <= 1; i++) {
			float cellOffset = i;
			float pointPosition = cellOffset + hash_float_to_float(cellPosition + cellOffset) * randomness;
		    float distanceToPoint = voronoi_distance_1d(pointPosition, localPosition, metric, exponent);
			if (distanceToPoint < minDistance) {
			  targetOffset = cellOffset;
			  minDistance = distanceToPoint; 
			  targetPosition = pointPosition;
			}
	  }

	  outColor = hash_float_to_float3(cellPosition + targetOffset);
      outDistance = minDistance;
      outW = targetPosition + cellPosition;
}

void Blender_Voronoi_float(float3 coord, float smoothness, float exponent, float randomness, int metric, out float outDistance, out float3 outColor, out float3 outPosition)
{
	float3 cellPosition = floor(coord);
	float3 localPosition = coord - cellPosition;

	float smoothDistance = 8.0f;
	float3 smoothColor = make_float3(0.0f, 0.0f, 0.0f);
	float3 smoothPosition = make_float3(0.0f, 0.0f, 0.0f);
	for (int k = -2; k <= 2; k++) {
		for (int j = -2; j <= 2; j++) {
			for (int i = -2; i <= 2; i++) {
				float3 cellOffset = make_float3(i, j, k);
				float3 pointPosition = cellOffset +
					hash_float3_to_float3(cellPosition + cellOffset) * randomness;
				float distanceToPoint = voronoi_distance_3d(
					pointPosition, localPosition, metric, exponent);
				float h = smoothstep(
					0.0f, 1.0f, 0.5f + 0.5f * (smoothDistance - distanceToPoint) / smoothness);
				float correctionFactor = smoothness * h * (1.0f - h);
				smoothDistance = lerp(smoothDistance, distanceToPoint, h) - correctionFactor;
				correctionFactor /= 1.0f + 3.0f * smoothness;
				float3 cellColor = hash_float3_to_float3(cellPosition + cellOffset);
				smoothColor = lerp(smoothColor, cellColor, h) - correctionFactor;
				smoothPosition = lerp(smoothPosition, pointPosition, h) - correctionFactor;
			}
		}
	}

	outColor = smoothColor;
	outDistance = smoothDistance;
	outPosition = cellPosition + smoothPosition;
}

void PixelShaderFunction_float(out float4 color, out bool test)
{
	color = float4(1, 1, 0, 1);
	test = true;
}