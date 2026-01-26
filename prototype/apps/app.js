function generateProfile() {
    document.getElementById('ai-profile-result').classList.remove('hidden');
}

function closeProfileResult() {
    document.getElementById('ai-profile-result').classList.add('hidden');
    window.location.href = 'manage-children.html';
}

function animateMascot(emotion) {
    const mascots = document.querySelectorAll('.child-mascot');
    mascots.forEach(mascot => {
        mascot.classList.remove('happy', 'sad');
        mascot.classList.add(emotion);
        setTimeout(() => {
            mascot.classList.remove(emotion);
        }, 600);
    });
}

function saveExpense() {
    alert('Great job tracking your spending! 🎉');
    window.location.href = 'child-dashboard.html';
}


function showStoryResult(choice) {
    const resultDiv = document.getElementById('story-result');
    const mascot = document.getElementById('result-mascot');
    const title = document.getElementById('result-title');
    const explanation = document.getElementById('result-explanation');
    const badge = document.getElementById('result-badge');
    const badgeText = document.getElementById('result-badge-text');

    resultDiv.classList.remove('hidden');

    switch (choice) {
        case 'smart':

            mascot.classList.add('happy');
            title.textContent = 'Excellent Choice! 🌟';
            explanation.textContent = 'Emma chose to save her money and add it to her art set fund. Now she has $30 total - enough to buy the art set AND have $5 left over! This shows great self-control and goal focus.';
            badge.textContent = '🏆';
            badgeText.textContent = 'You earned the "Smart Saver" badge!';
            break;

        case 'partial':

            title.textContent = 'Not Bad! 👍';
            explanation.textContent = 'Emma chose the cheaper option, which is better than the expensive one. But she could have saved all the money for her art set goal instead.';
            badge.textContent = '⭐';
            badgeText.textContent = 'You earned the "Budget Aware" badge!';
            break;
        case 'impulsive':

            mascot.classList.add('sad');
            title.textContent = 'Think Again! 🤔';
            explanation.textContent = 'Emma spent most of her money on something she wanted right now, but this means she\'s further from her art set goal. Sometimes it\'s better to wait!';
            badge.textContent = '📚';
            badgeText.textContent = 'You earned the "Learning" badge!';
            break;
    }

    setTimeout(() => {
        mascot.classList.remove('happy', 'sad');
    }, 600);
}

function resetStoryQuiz() {
    document.getElementById('story-result').classList.add('hidden');
    document.getElementById('result-mascot').classList.remove('happy', 'sad');
}

// Category and mood selection handlers
document.addEventListener('DOMContentLoaded', function () {
    // Category button selection
    document.querySelectorAll('.category-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            document.querySelectorAll('.category-btn').forEach(b => {
                b.classList.remove('bg-blue-200');
                b.classList.add('bg-gray-100');
            });
            this.classList.remove('bg-gray-100');
            this.classList.add('bg-blue-200');
        });
    });

    // Mood button selection
    document.querySelectorAll('.mood-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            document.querySelectorAll('.mood-btn').forEach(b => {
                b.classList.remove('bg-yellow-200');
                b.classList.add('bg-gray-100');
            });
            this.classList.remove('bg-gray-100');
            this.classList.add('bg-yellow-200');
        });
    });
});

// Report generation functions
function generateReport() {
    const reportType = document.getElementById('reportType').value;
    const previewDiv = document.querySelector('#reportPreview .bg-gray-50');

    let reportContent = '';

    switch (reportType) {
        case 'monthly':
            reportContent = `
                        <div class="text-left">
                            <h4 class="font-bold mb-2">December 2024 - Monthly Summary</h4>
                            <div class="space-y-2 text-sm">
                                <div>Total Spending: <strong>$67.50</strong></div>
                                <div>Number of Purchases: <strong>15</strong></div>
                                <div>Average per Purchase: <strong>$4.50</strong></div>
                                <div>Most Active Day: <strong>Saturday</strong></div>
                                <div>Top Category: <strong>Toys (60%)</strong></div>
                                <div>Dominant Mood: <strong>Happy (70%)</strong></div>
                            </div>
                        </div>
                    `;
            break;
        case 'category':
            reportContent = `
                        <div class="text-left">
                            <h4 class="font-bold mb-2">Category Analysis Report</h4>
                            <div class="space-y-2 text-sm">
                                <div>🧸 Toys: <strong>$40.50 (60%)</strong></div>
                                <div>🍭 Snacks: <strong>$16.88 (25%)</strong></div>
                                <div>📚 Books: <strong>$10.12 (15%)</strong></div>
                                <div class="mt-3 pt-2 border-t">
                                    <div><strong>Insights:</strong></div>
                                    <div>• Strong preference for toys</div>
                                    <div>• Balanced snack spending</div>
                                    <div>• Opportunity to increase book purchases</div>
                                </div>
                            </div>
                        </div>
                    `;
            break;
        case 'mood':
            reportContent = `
                        <div class="text-left">
                            <h4 class="font-bold mb-2">Mood-Based Spending Analysis</h4>
                            <div class="space-y-2 text-sm">
                                <div>😊 Happy: <strong>$47.25 (70%)</strong></div>
                                <div>😋 Excited: <strong>$13.50 (20%)</strong></div>
                                <div>😐 Neutral: <strong>$6.75 (10%)</strong></div>
                                <div class="mt-3 pt-2 border-t">
                                    <div><strong>Key Finding:</strong></div>
                                    <div>Emma spends 3x more when happy vs neutral</div>
                                </div>
                            </div>
                        </div>
                    `;
            break;
        case 'goals':
            reportContent = `
                        <div class="text-left">
                            <h4 class="font-bold mb-2">Goals Progress Report</h4>
                            <div class="space-y-2 text-sm">
                                <div>🎨 Art Set Goal: <strong>60% Complete ($15/$25)</strong></div>
                                <div>⭐ Mom's Challenge: <strong>70% Complete ($7/$10)</strong></div>
                                <div class="mt-3 pt-2 border-t">
                                    <div><strong>Achievements:</strong></div>
                                    <div>• 📚 Book Set Goal - Completed Dec 10</div>
                                    <div>• 💰 First $20 Saved - Completed Dec 5</div>
                                </div>
                            </div>
                        </div>
                    `;
            break;
        case 'complete':
            reportContent = `
                        <div class="text-left">
                            <h4 class="font-bold mb-2">Complete Financial Analysis</h4>
                            <div class="space-y-2 text-sm">
                                <div><strong>Spending Overview:</strong> $67.50 total, 15 purchases</div>
                                <div><strong>Top Category:</strong> Toys (60%, $40.50)</div>
                                <div><strong>Mood Pattern:</strong> 70% spending when happy</div>
                                <div><strong>Goals Status:</strong> 2 active, 2 completed</div>
                                <div><strong>Savings Rate:</strong> 35% of allowance saved</div>
                                <div class="mt-3 pt-2 border-t">
                                    <div><strong>Recommendations:</strong></div>
                                    <div>• Set happiness spending limits</div>
                                    <div>• Encourage diverse category spending</div>
                                    <div>• Celebrate goal achievements</div>
                                </div>
                            </div>
                        </div>
                    `;
            break;
    }

    previewDiv.innerHTML = reportContent;
    previewDiv.classList.remove('flex', 'items-center', 'justify-center', 'text-gray-500');
    previewDiv.classList.add('text-left');
}

function downloadPDF() {
    // Simulate PDF download
    const reportType = document.getElementById('reportType').value;
    const fileName = `Emma_${reportType}_Report_${new Date().toISOString().split('T')[0]}.pdf`;

    // Create a temporary link to simulate download
    const link = document.createElement('a');
    link.href = '#';
    link.download = fileName;

    // Show success message
    alert(`📄 PDF Report "${fileName}" would be downloaded!\n\nThis is a demo - in a real app, this would generate and download an actual PDF file.`);
}

// Child management functions
function editChild(childId) {
    alert(`Edit ${childId} - This would open a form to edit the child's information, change their name, age, or reset their login code.`);
}

function confirmDeleteChild(childId) {
    const childName = childId === 'emma' ? 'Emma' : 'Jake';
    if (confirm(`⚠️ Are you sure you want to delete ${childName}'s account?\n\nThis will permanently delete:\n• All spending history\n• All goals and achievements\n• All saved data\n\nThis action cannot be undone!`)) {
        alert(`${childName}'s account has been deleted.`);
        // In a real app, this would remove the child from the UI
    }
}

function confirmDeleteAccount() {
    if (confirm('⚠️ DELETE ENTIRE ACCOUNT?\n\nThis will permanently delete:\n• Your parent account\n• ALL children accounts\n• ALL financial data\n• ALL history and reports\n\nType "DELETE" to confirm this action.')) {
        const confirmation = prompt('Type "DELETE" to confirm:');
        if (confirmation === 'DELETE') {
            alert('Account deletion initiated. You will receive a confirmation email.');
        } else {
            alert('Account deletion cancelled.');
        }
    }
}

// Child switching functionality
let currentChild = 'emma';

function switchChild(childId) {
    currentChild = childId;

    // Update button states
    document.querySelectorAll('.child-btn').forEach(btn => {
        btn.classList.remove('bg-white', 'bg-opacity-20', 'font-medium');
        btn.classList.add('bg-white', 'bg-opacity-10');
    });

    const activeBtn = document.querySelector(`[data-child="${childId}"]`);
    if (activeBtn) {
        activeBtn.classList.remove('bg-white', 'bg-opacity-10');
        activeBtn.classList.add('bg-white', 'bg-opacity-20', 'font-medium');
    }

    // Update dashboard data based on selected child
    updateDashboardData(childId);
}

function updateDashboardData(childId) {
    const childData = {
        emma: {
            balance: '$47.50',
            weeklySpent: '$12.30',
            activities: [
                { icon: '🧸', name: 'Toy Store', time: 'Today, 2:30 PM • 😊 Happy', amount: '-$8.99' },
                { icon: '🍭', name: 'Candy', time: 'Yesterday • 😋 Excited', amount: '-$2.50' }
            ]
        },
        jake: {
            balance: '$23.75',
            weeklySpent: '$8.50',
            activities: [
                { icon: '🎮', name: 'Game Store', time: 'Today, 4:15 PM • 😊 Happy', amount: '-$15.99' },
                { icon: '🍕', name: 'Pizza', time: 'Yesterday • 😋 Hungry', amount: '-$7.50' }
            ]
        }
    };

    const data = childData[childId];
    if (data) {
        document.getElementById('current-balance').textContent = data.balance;
        document.getElementById('weekly-spent').textContent = data.weeklySpent;

        const activityContainer = document.getElementById('recent-activity');
        activityContainer.innerHTML = data.activities.map(activity => `
                    <div class="flex items-center justify-between">
                        <div class="flex items-center">
                            <span class="text-2xl mr-3">${activity.icon}</span>
                            <div>
                                <div class="font-medium">${activity.name}</div>
                                <div class="text-sm text-gray-500">${activity.time}</div>
                            </div>
                        </div>
                        <div class="text-red-500 font-semibold">${activity.amount}</div>
                    </div>
                `).join('');
    }
}

function addChildByCode() {
    // Get code from input field
    const codeInput = document.getElementById('childCodeInput');
    const code = codeInput ? codeInput.value.trim() : '';

    if (!code) {
        alert('Please enter a child code first!');
        return;
    }

    // Simulate looking up the code
    if (code.length >= 6) {
        alert(`✅ Found child with code: ${code}\n\nChild "Alex Johnson" has been added to your account!\n\nIn a real app, this would connect to an existing child account.`);
        // Clear the input
        if (codeInput) codeInput.value = '';
    } else {
        alert('❌ Invalid code format. Child codes are usually 6+ characters long.');
    }
}

function showNotifications() {
    // Create notification modal
    const modal = document.createElement('div');
    modal.className = 'fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50';
    modal.innerHTML = `
                <div class="bg-white rounded-2xl p-6 m-4 max-w-sm w-full">
                    <div class="flex items-center justify-between mb-4">
                        <h3 class="text-lg font-bold">🔔 Notifications</h3>
                        <button onclick="this.closest('.fixed').remove()" class="text-gray-500 text-xl">×</button>
                    </div>
                    <div class="space-y-3">
                        <div class="bg-green-50 p-3 rounded-xl border border-green-200">
                            <div class="flex items-start">
                                <span class="text-xl mr-2">🎉</span>
                                <div>
                                    <div class="font-medium text-green-700">Goal Completed!</div>
                                    <div class="text-sm text-gray-600">Emma reached her book savings goal</div>
                                    <div class="text-xs text-gray-500 mt-1">2 hours ago</div>
                                </div>
                            </div>
                        </div>
                        <div class="bg-yellow-50 p-3 rounded-xl border border-yellow-200">
                            <div class="flex items-start">
                                <span class="text-xl mr-2">💰</span>
                                <div>
                                    <div class="font-medium text-yellow-700">Large Purchase Alert</div>
                                    <div class="text-sm text-gray-600">Jake spent $15.99 on games</div>
                                    <div class="text-xs text-gray-500 mt-1">4 hours ago</div>
                                </div>
                            </div>
                        </div>
                        <div class="bg-blue-50 p-3 rounded-xl border border-blue-200">
                            <div class="flex items-start">
                                <span class="text-xl mr-2">📅</span>
                                <div>
                                    <div class="font-medium text-blue-700">Allowance Due</div>
                                    <div class="text-sm text-gray-600">Weekly allowance is due tomorrow</div>
                                    <div class="text-xs text-gray-500 mt-1">1 day</div>
                                </div>
                            </div>
                        </div>
                        <div class="bg-purple-50 p-3 rounded-xl border border-purple-200">
                            <div class="flex items-start">
                                <span class="text-xl mr-2">💡</span>
                                <div>
                                    <div class="font-medium text-purple-700">Parenting Tip</div>
                                    <div class="text-sm text-gray-600">Try the "24-hour rule" - have Emma wait a day before buying toys over $10</div>
                                    <div class="text-xs text-gray-500 mt-1">Daily tip</div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <button onclick="this.closest('.fixed').remove()" class="w-full bg-blue-500 text-white py-3 rounded-xl font-semibold mt-4">
                        Close
                    </button>
                </div>
            `;
    document.body.appendChild(modal);
}

function showChildNotifications() {
    // Create child-friendly notification modal
    const modal = document.createElement('div');
    modal.className = 'fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50';
    modal.innerHTML = `
                <div class="bg-white rounded-2xl p-6 m-4 max-w-sm w-full">
                    <div class="flex items-center justify-between mb-4">
                        <h3 class="text-lg font-bold">🔔 Your Messages!</h3>
                        <button onclick="this.closest('.fixed').remove()" class="text-gray-500 text-xl">×</button>
                    </div>
                    <div class="space-y-3">
                        <div class="bg-green-50 p-3 rounded-xl border border-green-200">
                            <div class="flex items-start">
                                <span class="text-2xl mr-2">🎉</span>
                                <div>
                                    <div class="font-medium text-green-700">Great Job!</div>
                                    <div class="text-sm text-gray-600">You're so close to your art set goal! Only $10 more to go!</div>
                                    <div class="text-xs text-gray-500 mt-1">1 hour ago</div>
                                </div>
                            </div>
                        </div>
                        <div class="bg-purple-50 p-3 rounded-xl border border-purple-200">
                            <div class="flex items-start">
                                <span class="text-2xl mr-2">💰</span>
                                <div>
                                    <div class="font-medium text-purple-700">Allowance Time!</div>
                                    <div class="text-sm text-gray-600">Your weekly allowance of $10 is ready!</div>
                                    <div class="text-xs text-gray-500 mt-1">Tomorrow</div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <button onclick="this.closest('.fixed').remove()" class="w-full bg-green-500 text-white py-3 rounded-xl font-semibold mt-4">
                        Got it! 😊
                    </button>
                </div>
            `;
    document.body.appendChild(modal);
}

// Avatar selection functions
function showAvatarSelector() {
    document.getElementById('avatar-selector').classList.remove('hidden');
}

function hideAvatarSelector() {
    document.getElementById('avatar-selector').classList.add('hidden');
}

function selectAvatar(emoji) {
    // Update only child-mascot elements
    document.querySelectorAll('.child-mascot').forEach(mascot => {
        mascot.textContent = emoji;
    });

    // Hide selector
    hideAvatarSelector();

    // Show success message
    alert(`Great choice! Your new avatar is ${emoji} 🌟`);
}

// Period switching functionality
function switchPeriod(period, buttonElement) {
    // Update button states
    document.querySelectorAll('.period-btn').forEach(btn => {
        btn.classList.remove('bg-blue-500', 'text-white');
        btn.classList.add('bg-gray-200', 'text-gray-700');
    });

    buttonElement.classList.remove('bg-gray-200', 'text-gray-700');
    buttonElement.classList.add('bg-blue-500', 'text-white');

    // Update chart data based on period
    updateChartData(period);
}

function updateChartData(period) {
    const chartTitle = document.getElementById('chart-title');
    const chartContainer = document.getElementById('spending-chart');
    const chartInsight = document.getElementById('chart-insight');

    const chartData = {
        weekly: {
            title: 'Weekly Spending Trend',
            insight: 'Peak spending on weekends ($14.74 vs $11.70 weekdays)',
            data: [
                { label: 'Mon', amount: '$3.20', height: '40px' },
                { label: 'Tue', amount: '$1.50', height: '16px' },
                { label: 'Wed', amount: '$4.75', height: '60px' },
                { label: 'Thu', amount: '$2.25', height: '24px' },
                { label: 'Fri', amount: '$6.50', height: '80px' },
                { label: 'Sat', amount: '$8.99', height: '96px' },
                { label: 'Sun', amount: '$5.75', height: '72px' }
            ]
        },
        monthly: {
            title: 'Monthly Spending Trend',
            insight: 'Highest spending in Week 3 ($28.50) - holiday shopping',
            data: [
                { label: 'Week 1', amount: '$15.25', height: '48px' },
                { label: 'Week 2', amount: '$18.75', height: '60px' },
                { label: 'Week 3', amount: '$28.50', height: '96px' },
                { label: 'Week 4', amount: '$22.30', height: '72px' }
            ]
        },
        alltime: {
            title: 'All Time Spending Trend',
            insight: 'Steady growth in spending habits - December peak due to holidays',
            data: [
                { label: 'Oct', amount: '$45.20', height: '36px' },
                { label: 'Nov', amount: '$52.75', height: '42px' },
                { label: 'Dec', amount: '$67.50', height: '54px' },
                { label: 'Jan', amount: '$38.90', height: '31px' },
                { label: 'Feb', amount: '$41.25', height: '33px' },
                { label: 'Mar', amount: '$58.60', height: '47px' },
                { label: 'Apr', amount: '$73.80', height: '59px' }
            ]
        }
    };

    const selectedData = chartData[period];

    // Update title and insight
    chartTitle.textContent = selectedData.title;
    chartInsight.textContent = selectedData.insight;

    // Update chart bars
    chartContainer.innerHTML = selectedData.data.map(item => `
                <div class="flex flex-col items-center">
                    <div class="bg-blue-500 w-6 rounded-t" style="height: ${item.height}"></div>
                    <span class="text-xs mt-1">${item.label}</span>
                    <span class="text-xs text-gray-500">${item.amount}</span>
                </div>
            `).join('');
}