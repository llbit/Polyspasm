#version 130

uniform usampler1D	perm;
uniform usampler1D	perm12;
uniform vec2		offset;
out vec4			fragColor;

const vec2 grad[12] = vec2[12](
	vec2(1,1),vec2(-1,1),vec2(1,-1),vec2(-1,-1),
    vec2(1,0),vec2(-1,0),vec2(1,0),vec2(-1,0),
    vec2(0,1),vec2(0,-1),vec2(0,1),vec2(0,-1)
);

const vec2 FG = vec2( 0.5*(sqrt(3.0)-1.0), (3.0-sqrt(3.0))/6.0 );

int permutation(ivec2 id)
{
	int	p0 = int(texelFetch(perm, id.y, 0).x);
	return int(texelFetch(perm12, id.x + p0, 0).x);
}

/* 2D simplex noise (improved perlin noise)
 * Based on article and code by Stefan Gustavson:
 * http://www.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf
 */
float simplex(vec2 x)
{
	float	u = dot(x, FG.xx);
	ivec2	i = ivec2(floor(x + u));
	float	v = dot(i, FG.yy);
	vec2	x0 = x - (i-v);
	ivec2	i1;

	if (x0.x>x0.y)
		i1 = ivec2(1, 0);
	else
		i1 = ivec2(0, 1);

	vec2	x1 = x0 - i1 + FG.yy;
	vec2	x2 = x0 + vec2(FG.y * 2.0 - 1.0);

	ivec2	id = ivec2(i % 256);
	ivec3	gi = ivec3(permutation(id), permutation(id + i1),
		permutation(id + ivec2(1)));

	vec3	n;
	vec3	t = vec3(0.5) - vec3(dot(x0,x0), dot(x1,x1), dot(x2,x2));

#if 1
	if (t.x < 0.0) {
		n.x = 0.0;
	} else {
		t.x *= t.x;
		n.x = t.x * t.x * dot(grad[gi.x], x0);
	}
#else
	n.x = 0.0;
#endif

#if 1
	if (t.y < 0.0) {
		n.y = 0.0;
	} else {
		t.y *= t.y;
		n.y = t.y * t.y * dot(grad[gi.y], x1);
	}
#else
	n.y = 0.0;
#endif

#if 1
	if (t.z < 0.0) {
		n.z = 0.0;
	} else {
		t.z *= t.z;
		n.z = t.z * t.z * dot(grad[gi.z], x2);
	}
#else
	n.z = 0.0;
#endif

	return (n.x+n.y+n.z) * 35.0 + 0.5;
}

void main()
{
	vec2	x = gl_FragCoord.xy * 0.025;// + offset;
	float	value = 5.0 * pow(simplex(x), 6.0);
	fragColor = vec4(value);
}
