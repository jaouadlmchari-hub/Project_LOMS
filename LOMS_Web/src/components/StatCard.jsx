export default function StatCard({ title, value, icon, color = 'blue', subtitle }) {
  const colors = {
    blue: { bg: 'bg-blue-50', icon: 'bg-blue-600', text: 'text-blue-600' },
    green: { bg: 'bg-green-50', icon: 'bg-green-600', text: 'text-green-600' },
    purple: { bg: 'bg-purple-50', icon: 'bg-purple-600', text: 'text-purple-600' },
    orange: { bg: 'bg-orange-50', icon: 'bg-orange-600', text: 'text-orange-600' },
  }
  const c = colors[color] || colors.blue

  return (
    <div className="card flex items-center gap-4">
      <div className={`${c.icon} p-3 rounded-xl text-white flex-shrink-0`}>
        {icon}
      </div>
      <div className="min-w-0">
        <p className="text-sm text-gray-500 font-medium">{title}</p>
        <p className="text-2xl font-bold text-gray-800">{value ?? '—'}</p>
        {subtitle && <p className="text-xs text-gray-400 mt-0.5">{subtitle}</p>}
      </div>
    </div>
  )
}
