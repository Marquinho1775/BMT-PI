export function getToken() {
    return localStorage.getItem('token');
}

export function getUser() {
    const userData = localStorage.getItem('user');
    if (userData) {
        try {
            return JSON.parse(userData);
        } catch (error) {
            console.error("Failed to parse user data from local storage:", error);
            localStorage.removeItem('user');
            return null;
        }
    }
    return null;
}

export function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
}
