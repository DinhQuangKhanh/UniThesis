import { useState, useEffect } from "react";
import { NotificationDropdown } from "@/components/layout";
import { useSystemError } from "@/contexts/SystemErrorContext";
import { studentGroupService, OpenGroupDto, PendingJoinRequestDto } from "@/lib/studentGroupService";

export function StudentOpenGroupsPage() {
  const { showError } = useSystemError();
  const [openGroups, setOpenGroups] = useState<OpenGroupDto[]>([]);
  const [myGroup, setMyGroup] = useState<{ groupCode: string; groupName?: string } | null>(null);
  const [loading, setLoading] = useState(true);
  const [selectedGroup, setSelectedGroup] = useState<OpenGroupDto | null>(null);
  const [showRequestModal, setShowRequestModal] = useState(false);
  const [requestMessage, setRequestMessage] = useState("");
  const [requesting, setRequesting] = useState(false);
  const [pendingJoinRequest, setPendingJoinRequest] = useState<PendingJoinRequestDto | null>(null);
  const [toastMessage, setToastMessage] = useState<string | null>(null);

  useEffect(() => {
    fetchOpenGroups();
  }, []);

  const fetchOpenGroups = async () => {
    try {
      setLoading(true);
      const [groups, pending, currentGroup] = await Promise.all([
        studentGroupService.getOpenGroups(),
        studentGroupService.getMyPendingJoinRequest(),
        studentGroupService.getMyGroup(),
      ]);
      setOpenGroups(groups);
      setPendingJoinRequest(pending);
      setMyGroup(currentGroup ? { groupCode: currentGroup.groupCode, groupName: currentGroup.groupName } : null);
    } catch (err) {
      showError(err instanceof Error ? err.message : "Có lỗi khi tải dữ liệu");
      console.error("Error fetching open groups:", err);
    } finally {
      setLoading(false);
    }
  };

  const hasGroupInCurrentSemester = !!myGroup;

  const handleRequestJoin = async () => {
    if (!selectedGroup || pendingJoinRequest) return;
    try {
      setRequesting(true);
      await studentGroupService.requestJoin(selectedGroup.groupId, requestMessage || undefined);
      setShowRequestModal(false);
      setRequestMessage("");
      setSelectedGroup(null);
      setToastMessage("Gửi yêu cầu thành công, hãy chờ nhóm trưởng phê duyệt yêu cầu của bạn");
      setTimeout(() => setToastMessage(null), 4000);
      // Refresh the list
      await fetchOpenGroups();
    } catch (err) {
      showError(err instanceof Error ? err.message : "Không thể gửi yêu cầu");
    } finally {
      setRequesting(false);
    }
  };

  return (
    <div className="flex flex-col h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white border-b border-gray-200 px-8 py-4 flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-800">Nhóm khác</h1>
        <NotificationDropdown role="student" isNavy={true} />
      </header>

      {/* Main Content */}
      <main className="flex-1 overflow-y-auto p-8">
        {toastMessage && (
          <div className="fixed top-6 right-6 z-50 bg-green-600 text-white px-5 py-3 rounded-lg shadow-lg max-w-md">
            {toastMessage}
          </div>
        )}

        {pendingJoinRequest && (
          <div className="mb-6 bg-amber-50 border border-amber-200 text-amber-800 px-4 py-3 rounded-lg">
            <p className="font-semibold">Bạn đang chờ phê duyệt yêu cầu tham gia nhóm</p>
            <p className="text-sm mt-1">
              Nhóm: {pendingJoinRequest.groupName || pendingJoinRequest.groupCode} • Hết hạn lúc{" "}
              {new Date(pendingJoinRequest.expiresAt).toLocaleString("vi-VN")}
            </p>
          </div>
        )}

        {hasGroupInCurrentSemester && (
          <div className="mb-6 bg-slate-100 border border-slate-300 text-slate-700 px-4 py-3 rounded-lg">
            <p className="font-semibold">Bạn đã có nhóm ở kỳ này</p>
            <p className="text-sm mt-1">Nhóm hiện tại: {myGroup.groupName || myGroup.groupCode}</p>
          </div>
        )}

        {loading ? (
          <div className="flex items-center justify-center h-96">
            <div className="text-gray-500">Đang tải dữ liệu...</div>
          </div>
        ) : openGroups.length === 0 ? (
          <div className="max-w-2xl mx-auto">
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-8 text-center">
              <div className="text-5xl mb-4">🔍</div>
              <h2 className="text-2xl font-bold text-gray-800 mb-2">Không có nhóm mở</h2>
              <p className="text-gray-500">
                Hiện tại không có nhóm mới để tham gia. Hãy kiểm tra lại sau hoặc tạo nhóm của riêng bạn.
              </p>
            </div>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {openGroups.map((group) => (
              <div
                key={group.groupId}
                className="bg-white rounded-lg shadow-sm border border-gray-200 hover:shadow-md transition p-6"
              >
                <div className="mb-4">
                  <h3 className="text-lg font-bold text-gray-800 mb-1">{group.groupName || group.groupCode}</h3>
                  <p className="text-sm text-gray-600">Mã: {group.groupCode}</p>
                </div>

                <div className="mb-4 p-3 bg-blue-50 rounded-lg">
                  <div className="flex items-center justify-between">
                    <span className="text-sm text-gray-700">Số thành viên</span>
                    <span className="text-xl font-bold text-blue-600">
                      {group.memberCount}/{group.maxMembers}
                    </span>
                  </div>
                  <div className="w-full bg-gray-200 rounded-full h-2 mt-2">
                    <div
                      className="bg-blue-600 h-2 rounded-full transition-all"
                      style={{ width: `${(group.memberCount / group.maxMembers) * 100}%` }}
                    />
                  </div>
                </div>

                {group.members && group.members.length > 0 && (
                  <div className="mb-4">
                    <p className="text-sm font-semibold text-gray-700 mb-2">Thành viên hiện tại:</p>
                    <div className="space-y-1">
                      {group.members.slice(0, 3).map((member) => (
                        <div key={member.studentId} className="text-sm text-gray-600">
                          <span className="font-medium">{member.fullName}</span>
                          <span className="text-gray-500"> ({member.studentCode})</span>
                        </div>
                      ))}
                      {group.members.length > 3 && (
                        <p className="text-sm text-gray-500 italic">...và {group.members.length - 3} người khác</p>
                      )}
                    </div>
                  </div>
                )}

                <p className="text-xs text-gray-500 mb-4">
                  Tạo ngày: {new Date(group.createdAt).toLocaleDateString("vi-VN")}
                </p>

                <button
                  onClick={() => {
                    if (pendingJoinRequest || hasGroupInCurrentSemester) return;
                    setSelectedGroup(group);
                    setShowRequestModal(true);
                  }}
                  disabled={!!pendingJoinRequest || hasGroupInCurrentSemester}
                  className="w-full bg-primary text-white px-4 py-2 rounded-lg font-semibold hover:bg-primary/90 disabled:bg-gray-300 disabled:cursor-not-allowed transition"
                >
                  {hasGroupInCurrentSemester
                    ? "Bạn đã có nhóm ở kỳ này"
                    : pendingJoinRequest
                      ? "Đang chờ phê duyệt"
                      : "Yêu cầu tham gia"}
                </button>
              </div>
            ))}
          </div>
        )}
      </main>

      {/* Request Modal */}
      {showRequestModal && selectedGroup && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-lg shadow-lg max-w-md w-full p-6">
            <h3 className="text-xl font-bold text-gray-800 mb-2">Yêu cầu tham gia nhóm</h3>
            <p className="text-gray-600 mb-4">{selectedGroup.groupName || selectedGroup.groupCode}</p>

            <div className="mb-6">
              <label className="block text-sm font-medium text-gray-700 mb-2">Lời nhắn (tùy chọn)</label>
              <textarea
                value={requestMessage}
                onChange={(e) => setRequestMessage(e.target.value)}
                placeholder="Giới thiệu về bản thân hoặc để trống..."
                maxLength={500}
                rows={4}
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
              />
              <p className="text-xs text-gray-500 mt-1">{requestMessage.length}/500</p>
            </div>

            <div className="flex gap-3">
              <button
                onClick={() => {
                  setShowRequestModal(false);
                  setRequestMessage("");
                  setSelectedGroup(null);
                }}
                className="flex-1 px-4 py-2 bg-gray-100 text-gray-700 rounded-lg font-semibold hover:bg-gray-200 transition"
              >
                Hủy
              </button>
              <button
                onClick={handleRequestJoin}
                disabled={requesting}
                className="flex-1 px-4 py-2 bg-primary text-white rounded-lg font-semibold hover:bg-primary/90 disabled:opacity-50 transition"
              >
                {requesting ? "Đang gửi..." : "Gửi yêu cầu"}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
