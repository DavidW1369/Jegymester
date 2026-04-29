import { useState } from 'react';
import api from '../api';
import { useNavigate } from 'react-router-dom';

export default function Register() {
    const [form, setForm] = useState({
        name: '',
        email: '',
        password: '',
        phoneNumber: ''
    });
    const navigate = useNavigate();

    const handleRegister = async (e) => {
        e.preventDefault(); // Megakadályozza az oldal újratöltését
        try {
            // A beküldött mezőknek egyezniük kell a RegisterDto-val!
            await api.post('/users/register', form);
            alert("Sikeres regisztráció!");
            navigate('/'); // Regisztráció után visszavisz a főoldalra
        } catch (err) {
            alert("Hiba: " + (err.response?.data?.message || "Sikertelen regisztráció"));
        }
    };

    return (
        <div style={{ padding: '20px', maxWidth: '400px', margin: '0 auto', color: 'white' }}>
            <h2>Regisztráció</h2>
            <form onSubmit={handleRegister} style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
                <input
                    placeholder="Teljes név"
                    onChange={e => setForm({ ...form, name: e.target.value })}
                    style={inputStyle} required
                />
                <input
                    type="email"
                    placeholder="E-mail cím"
                    onChange={e => setForm({ ...form, email: e.target.value })}
                    style={inputStyle} required
                />
                <input
                    type="tel"
                    placeholder="Telefonszám (pl. +36301234567)"
                    onChange={e => setForm({ ...form, phoneNumber: e.target.value })}
                    style={inputStyle} required
                />
                <input
                    type="password"
                    placeholder="Jelszó"
                    onChange={e => setForm({ ...form, password: e.target.value })}
                    style={inputStyle} required
                />
                <button type="submit" style={btnStyle}>Fiók létrehozása</button>
            </form>
        </div>
    );
}

const inputStyle = { padding: '10px', borderRadius: '4px', border: '1px solid #444', background: '#222', color: 'white' };
const btnStyle = { padding: '10px', background: '#28a745', color: 'white', border: 'none', cursor: 'pointer', fontWeight: 'bold' };