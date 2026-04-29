import { useEffect, useState } from 'react';
import api from '../api';

export default function AdminDashboard() {
    const [movies, setMovies] = useState([]);
    const [rooms, setRooms] = useState([]);

    // Kibővített Movie state
    const [newMovie, setNewMovie] = useState({
        name: "",
        description: "",
        length: 120,
        genre: 0 // Thriller az alapértelmezett (0. index)
    });

    // Screening state
    const [newScreening, setNewScreening] = useState({
        movieId: "",
        roomId: "",
        startTime: ""
    });

    useEffect(() => {
        loadData();
    }, []);

    const loadData = async () => {
        try {
            const mRes = await api.get('/admin/movies');
            const rRes = await api.get('/admin/rooms');
            setMovies(mRes.data);
            setRooms(rRes.data);
        } catch (err) { console.error("Hiba az adatok betöltésekor", err); }
    };

    const handleAddMovie = async () => {
        try {
            // Fontos: a számokat (length, genre) számmá alakítjuk
            await api.post('/admin/add-movie', {
                ...newMovie,
                length: parseFloat(newMovie.length),
                genre: parseInt(newMovie.genre)
            });
            alert("Film sikeresen hozzáadva!");
            setNewMovie({ name: "", description: "", length: 120, genre: 0 });
            loadData();
        } catch (err) {
            alert("Hiba: " + JSON.stringify(err.response?.data?.errors || err.response?.data));
        }
    };

    const handleCreateScreening = async () => {
        try {
            await api.post('/admin/create-screening', {
                movieId: parseInt(newScreening.movieId),
                roomId: parseInt(newScreening.roomId),
                startTime: newScreening.startTime
            });
            alert("Vetítés sikeresen létrehozva!");
            loadData();
        } catch (err) {
            alert("Hiba: " + (err.response?.data?.message || "Ütközés vagy hibás adatok!"));
        }
    };

    const handleDeleteMovie = async (id) => {
        if (!window.confirm("Biztosan törlöd?")) return;
        try {
            await api.delete(`/admin/delete-movie/${id}`);
            loadData();
        } catch (err) { alert(err.response?.data?.message); }
    };

    return (
        <div style={{ color: 'white', padding: '20px', maxWidth: '1200px', margin: '0 auto' }}>
            <h1 style={{ textAlign: 'center', marginBottom: '40px' }}>⚙️ Adminisztrációs Panel</h1>

            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '30px' }}>

                {/* 1. FILM FELVITELE */}
                <section style={sectionStyle}>
                    <h2>🎬 Új film hozzáadása</h2>
                    <input style={inputStyle} placeholder="Film címe" value={newMovie.name} onChange={e => setNewMovie({ ...newMovie, name: e.target.value })} />

                    <textarea style={{ ...inputStyle, height: '80px' }} placeholder="Leírás" value={newMovie.description} onChange={e => setNewMovie({ ...newMovie, description: e.target.value })} />

                    <div style={{ display: 'flex', gap: '10px' }}>
                        <div style={{ flex: 1 }}>
                            <label style={labelStyle}>Hossz (perc):</label>
                            <input type="number" style={inputStyle} value={newMovie.length} onChange={e => setNewMovie({ ...newMovie, length: e.target.value })} />
                        </div>
                        <div style={{ flex: 1 }}>
                            <label style={labelStyle}>Műfaj:</label>
                            <select style={inputStyle} value={newMovie.genre} onChange={e => setNewMovie({ ...newMovie, genre: e.target.value })}>
                                <option value="0">Thriller</option>
                                <option value="1">Science Fiction</option>
                                <option value="2">Western</option>
                                <option value="3">Fantasy</option>
                            </select>
                        </div>
                    </div>

                    <button style={addBtnStyle} onClick={handleAddMovie}>Film mentése</button>

                    <h3 style={{ marginTop: '30px' }}>Jelenlegi filmek</h3>
                    <div style={{ maxHeight: '200px', overflowY: 'auto', background: '#111', padding: '10px', borderRadius: '5px' }}>
                        {movies.map(m => (
                            <div key={m.id} style={listItemStyle}>
                                <span>{m.name} ({m.length} p)</span>
                                <button onClick={() => handleDeleteMovie(m.id)} style={delBtnStyle}>✖</button>
                            </div>
                        ))}
                    </div>
                </section>

                {/* 2. VETÍTÉS LÉTREHOZÁSA */}
                <section style={sectionStyle}>
                    <h2>📅 Új vetítés kiírása</h2>

                    <label style={labelStyle}>Válassz filmet:</label>
                    <select style={inputStyle} value={newScreening.movieId} onChange={e => setNewScreening({ ...newScreening, movieId: e.target.value })}>
                        <option value="">-- Válassz --</option>
                        {movies.map(m => <option key={m.id} value={m.id}>{m.name}</option>)}
                    </select>

                    <label style={labelStyle}>Válassz termet:</label>
                    <select style={inputStyle} value={newScreening.roomId} onChange={e => setNewScreening({ ...newScreening, roomId: e.target.value })}>
                        <option value="">-- Válassz --</option>
                        {rooms.map(r => <option key={r.id} value={r.id}>{r.id}. terem ({r.capacity} fő)</option>)}
                    </select>

                    <label style={labelStyle}>Kezdési időpont:</label>
                    <input type="datetime-local" style={inputStyle} value={newScreening.startTime} onChange={e => setNewScreening({ ...newScreening, startTime: e.target.value })} />

                    <button style={screenBtnStyle} onClick={handleCreateScreening}>Vetítés rögzítése</button>

                    <p style={{ fontSize: '0.8rem', color: '#888', marginTop: '10px' }}>
                        * A rendszer automatikusan ellenőrzi az ütközéseket (3 órás ablak).
                    </p>
                </section>
            </div>
        </div>
    );
}

// --- STÍLUSOK ---
const sectionStyle = { background: '#1a1a1a', padding: '25px', borderRadius: '15px', border: '1px solid #333' };
const labelStyle = { display: 'block', fontSize: '0.85rem', color: '#888', marginBottom: '5px' };
const inputStyle = { width: '100%', padding: '12px', marginBottom: '15px', background: '#2b2b2b', color: 'white', border: '1px solid #444', borderRadius: '6px', boxSizing: 'border-box' };
const addBtnStyle = { width: '100%', padding: '15px', background: '#28a745', color: 'white', border: 'none', borderRadius: '6px', fontWeight: 'bold', cursor: 'pointer' };
const screenBtnStyle = { width: '100%', padding: '15px', background: '#ffc107', color: 'black', border: 'none', borderRadius: '6px', fontWeight: 'bold', cursor: 'pointer' };
const listItemStyle = { display: 'flex', justifyContent: 'space-between', padding: '8px', borderBottom: '1px solid #222', fontSize: '0.9rem' };
const delBtnStyle = { background: 'none', border: 'none', color: '#ff4444', cursor: 'pointer', fontWeight: 'bold' };