/** @type {import('tailwindcss').Config} */
export default {
    content: [
        "./index.html",
        "./src/**/*.{js,ts,jsx,tsx}",
    ],
    theme: {
        extend: {
            colors: {
                "primary": "#2c6090",
                "primary-light": "#4682b4",
                "primary-dark": "#1f8b95",
                "success": "#5F8F61",
                "error": "#A64B4B",
                "warning": "#eab308",
                "navy-header": "#1e3a8a",
            },
            fontFamily: {
                "display": ["Inter", "sans-serif"],
                "body": ["Inter", "sans-serif"],
            },
            borderRadius: {
                "DEFAULT": "0.25rem",
                "md": "0.5rem",
                "lg": "0.5rem",
                "xl": "0.75rem",
                "full": "9999px"
            },
            boxShadow: {
                "soft": "0 20px 40px -5px rgba(40, 177, 189, 0.1), 0 10px 20px -5px rgba(0, 0, 0, 0.04)",
                "bento": "0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px -1px rgba(0, 0, 0, 0.1)",
            }
        },
    },
    plugins: [],
}
