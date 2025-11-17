/* Dashboard Container */
.dashboard - container {
    min - height: 100vh;
display: flex;
    flex - direction: column;
background: linear - gradient(135deg, #800000 0%, #ffffff 100%);
}

/* Header Styling */
.dashboard - header {
    text - align: center;
padding: 5rem 1rem;
color: white;
}

.dashboard - header h1 {
    font-size: 2.5rem;
font - weight: 700;
margin - bottom: 0.5rem;
}

.dashboard - header h4 {
    font-weight: 500;
color: #fff8b5;
}

/* Cards Styling */
.card {
    background-color: rgba(255, 255, 255, 0.9);
border: none;
border - radius: 1rem;
}

/* Buttons Styling */
.btn - dashboard {
background: linear - gradient(135deg, #FFD700 0%, #ffffff 100%);
    border: none;
color: #800000 !important;
    font - weight: 600;
    border - radius: 12px;
transition: all 0.3s ease-in-out;
}

.btn - dashboard:hover {
    transform: translateY(-2px);
box - shadow: 0 6px 16px rgba(0,0,0,0.25);
}

/* Footer Styling */
footer {
    margin-top: auto;
background: rgba(128, 0, 0, 0.8);
text - align: center;
color: white;
padding: 1rem 0;
}

/* Responsive adjustments */
@media screen and (max-width: 768px) {
    .dashboard-header h1 {
        font-size: 2rem;
    }
    .dashboard - header h4 {
        font-size: 1rem;
    }
}
