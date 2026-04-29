import { createBrowserRouter, Link, Outlet, useNavigate } from "react-router-dom";
import Home from "./Routes/Home";
import Register from "./Routes/Register";
import Login from "./Routes/Login";
import MyTickets from "./Routes/MyTickets";
import CheckTicket from "./Routes/CheckTicket";
import AdminDashboard from "./Routes/AdminDashboard";

const Layout = () => {
    const navigate = useNavigate();
    const user = JSON.parse(localStorage.getItem('user'));

    const handleLogout = () => {
        if (window.confirm("Biztosan ki szeretnél jelentkezni?")) {
            localStorage.removeItem('user');
            alert("Sikeres kijelentkezés!");
            navigate('/');
            window.location.reload();
        }
    };

    return (
        <div style={{ minHeight: '100vh', background: '#121212', color: 'white', fontFamily: 'Segoe UI, Tahoma, Geneva, Verdana, sans-serif' }}>
            {/* --- NAVIGÁCIÓS SÁV --- */}
            <nav style={{
                padding: '15px 30px',
                background: '#1a1a1a',
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center',
                borderBottom: '1px solid #333',
                position: 'sticky',
                top: 0,
                zIndex: 1000
            }}>
                <div style={{ display: 'flex', gap: '25px', alignItems: 'center' }}>
                    <Link to="/" style={navLinkStyle}>🎬 MoziFőoldal</Link>

                    {/* Bejelentkezett felhasználói menü */}
                    {user && <Link to="/my-tickets" style={navLinkStyle}>🎟️ Jegyeim</Link>}

                    {/* Pénztáros funkció (Role 1) */}
                    {user?.roleId === 1 && (
                        <Link to="/check-ticket" style={{ ...navLinkStyle, color: '#ffc107', fontWeight: 'bold' }}>
                            🔍 Beléptetés
                        </Link>
                    )}

                    {/* Admin funkció (Role 2) */}
                    {user?.roleId === 2 && (
                        <Link to="/admin" style={{ ...navLinkStyle, color: '#00d4ff', fontWeight: 'bold' }}>
                            ⚙️ Admin Panel
                        </Link>
                    )}
                </div>

                <div style={{ display: 'flex', gap: '20px', alignItems: 'center' }}>
                    {user ? (
                        <>
                            <span style={{ color: '#aaa' }}>Üdv, <strong style={{ color: 'white' }}>{user.name}</strong>!</span>
                            <button onClick={handleLogout} style={logoutBtnStyle}>Kijelentkezés</button>
                        </>
                    ) : (
                        <>
                            <Link to="/login" style={navLinkStyle}>Belépés</Link>
                            <Link to="/register" style={regBtnStyle}>Regisztráció</Link>
                        </>
                    )}
                </div>
            </nav>

            {/* --- TARTALOM --- */}
            <main style={{ padding: '20px' }}>
                <Outlet />
            </main>
        </div>
    );
};

// Stílusok
const navLinkStyle = { color: 'white', textDecoration: 'none', fontWeight: '500' };
const logoutBtnStyle = { background: '#dc3545', color: 'white', border: 'none', padding: '8px 16px', borderRadius: '4px', cursor: 'pointer', fontWeight: 'bold' };
const regBtnStyle = { background: '#28a745', color: 'white', textDecoration: 'none', padding: '8px 16px', borderRadius: '4px', fontWeight: 'bold' };

export const router = createBrowserRouter([
    {
        path: "/",
        element: <Layout />,
        children: [
            { path: "/", element: <Home /> },
            { path: "/register", element: <Register /> },
            { path: "/login", element: <Login /> },
            { path: "/my-tickets", element: <MyTickets /> },
            { path: "/check-ticket", element: <CheckTicket /> },
            { path: "/admin", element: <AdminDashboard /> }
        ]
    }
]);