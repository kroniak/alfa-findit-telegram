export const URL = process.env.NODE_ENV === "production" ? "https://bot.kroniak.net" : "http://localhost:5000";
export const AUTH_URL = URL + "/auth/login";
export const VERIFY_URL = URL + "/auth/verify";