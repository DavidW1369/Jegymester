import { useState } from 'react';
import api from '../api';
import { useNavigate } from 'react-router-dom';

export default function Login() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const navigate = useNavigate();

    const handleLogin = async (e) => {
        e.preventDefault();
        try {
            const res = await api.post('/users/login', { email, password });
            // Elmentjük a user adatait a böngészőbe
            localStorage.setItem('user', JSON.stringify(res.data));
            alert("Sikeres belépés!");
            window.location.href = "/"; // Frissítjük az oldalt, hogy a Layout is lássa
        } catch (err) {
            alert(err.response?.data?.message || "Hiba");
        }
    };

    return (
        <div style={{ padding: '20px', color: 'white', maxWidth: '300px', margin: '0 auto' }}>
            <h2>Bejelentkezés</h2>
            <form onSubmit={handleLogin} style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
                <input type="email" placeholder="Email" onChange={e => setEmail(e.target.value)} required />
                <input type="password" placeholder="Jelszó" onChange={e => setPassword(e.target.value)} required />
                <button type="submit">Belépés</button>
            </form>
        </div>
    );
}