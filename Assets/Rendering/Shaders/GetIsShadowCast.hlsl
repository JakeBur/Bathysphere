void GetIsShadowCast_float(out float value)
{
#ifdef UNIVERSAL_SHADOW_CASTER_PASS_INCLUDED
	value = 0;
#else
	value = 1;
#endif

}