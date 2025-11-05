shader_type canvas_item;

uniform float radius = 0.3; // Radio del aro (0–0.5 aprox)
uniform float width = 0.02; // Grosor del borde
uniform vec4 color : source_color;
uniform bool fade = true;

void fragment() {
    vec2 uv = UV - vec2(0.5);
    float dist = length(uv);

    float inner = radius - width * 0.5;
    float outer = radius + width * 0.5;

    float ring = smoothstep(outer, outer - 0.01, dist) * (1.0 - smoothstep(inner, inner - 0.01, dist));

    COLOR = color * ring;
    if (fade) COLOR.a *= 1.0 - radius * 2.0; // se desvanece un poco al crecer
}
