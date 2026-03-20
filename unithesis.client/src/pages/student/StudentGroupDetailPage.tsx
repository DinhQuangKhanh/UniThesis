import { useState, useEffect } from "react";
import { NotificationDropdown } from "@/components/layout";
import { useSystemError } from "@/contexts/SystemErrorContext";
import { SuccessModal } from "@/components/common/SuccessModal";
import { studentGroupService, StudentGroupDto, JoinRequestDto, PendingJoinRequestDto } from "@/lib/studentGroupService";

export function StudentGroupDetailPage() {
  const { showError } = useSystemError();
  const [myGroup, setMyGroup] = useState<StudentGroupDto | null>(null);
  const [joinRequests, setJoinRequests] = useState<JoinRequestDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [showInviteModal, setShowInviteModal] = useState(false);
  const [inviteData, setInviteData] = useState({ studentCode: "", message: "" });
  const [inviting, setInviting] = useState(false);
  const [creatingGroup, setCreatingGroup] = useState(false);
  const [groupName, setGroupName] = useState("");
  const [pendingJoinRequest, setPendingJoinRequest] = useState<PendingJoinRequestDto | null>(null);
  const [showSuccessModal, setShowSuccessModal] = useState(false);
  const [successMessage, setSuccessMessage] = useState("");

  useEffect(() => {
    fetchGroupData();
  }, []);

  const fetchGroupData = async () => {
    try {
      setLoading(true);
      const [group, pendingRequest] = await Promise.all([
        studentGroupService.getMyGroup(),
        studentGroupService.getMyPendingJoinRequest(),
      ]);
      setPendingJoinRequest(pendingRequest);

      if (group && group.groupId) {
        setMyGroup(group);
        try {
          const requests = await studentGroupService.getJoinRequests(group.groupId);
          setJoinRequests(requests);
        } catch (err) {
          console.error("Failed to fetch join requests:", err);
          setJoinRequests([]);
        }
      } else {
        setMyGroup(null);
        setJoinRequests([]);
      }
    } catch (err) {
      showError(err instanceof Error ? err.message : "Có lỗi khi tải dữ liệu");
      console.error("Error fetching group:", err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateGroup = async () => {
    if (pendingJoinRequest) {
      showError("Bạn đang có yêu cầu tham gia chờ phê duyệt, chưa thể tạo nhóm mới.");
      return;
    }

    try {
      setCreatingGroup(true);
      const result = await studentGroupService.createGroup(groupName || undefined);
      if (result.id) {
        fetchGroupData();
        setGroupName("");
      }
    } catch (err) {
      showError(err instanceof Error ? err.message : "Không thể tạo nhóm");
    } finally {
      setCreatingGroup(false);
    }
  };

  const handleInviteMember = async () => {
    if (!myGroup || !inviteData.studentCode) return;
    try {
      setInviting(true);
      await studentGroupService.inviteMember(myGroup.groupId, inviteData.studentCode, inviteData.message || undefined);
      setShowInviteModal(false);
      setInviteData({ studentCode: "", message: "" });
      setSuccessMessage(`Đã mời sinh viên ${inviteData.studentCode.toUpperCase()} thành công. Hãy chờ phản hồi từ họ.`);
      setShowSuccessModal(true);
      fetchGroupData();
    } catch (err) {
      showError(err instanceof Error ? err.message : "Không thể gửi lời mời");
    } finally {
      setInviting(false);
    }
  };

  const handleApproveRequest = async (requestId: number) => {
    if (!myGroup) return;
    try {
      await studentGroupService.approveJoinRequest(myGroup.groupId, requestId);
      fetchGroupData();
    } catch (err) {
      showError(err instanceof Error ? err.message : "Không thể chấp nhận yêu cầu");
    }
  };

  const handleRejectRequest = async (requestId: number) => {
    if (!myGroup) return;
    try {
      await studentGroupService.rejectJoinRequest(myGroup.groupId, requestId);
      fetchGroupData();
    } catch (err) {
      showError(err instanceof Error ? err.message : "Không thể từ chối yêu cầu");
    }
  };

  return (
    <div className="flex flex-col h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white border-b border-gray-200 px-8 py-4 flex items-center justify-between">
        <h1 className="text-2xl font-bold text-gray-800">Nhóm của tôi</h1>
        <NotificationDropdown role="student" isNavy={true} />
      </header>

      {/* Main Content */}
      <main className="flex-1 overflow-y-auto p-8">
        {loading ? (
          <div className="flex items-center justify-center h-96">
            <div className="text-gray-500">Đang tải...</div>
          </div>
        ) : !myGroup ? (
          <div className="max-w-2xl mx-auto">
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-8 text-center">
              <div className="text-5xl mb-4">👥</div>
              <h2 className="text-2xl font-bold text-gray-800 mb-2">Bạn chưa có nhóm</h2>
              <p className="text-gray-500 mb-6">Tạo nhóm của bạn ngay để bắt đầu hợp tác với những sinh viên khác</p>

              {pendingJoinRequest && (
                <div className="mb-6 bg-amber-50 border border-amber-200 text-amber-800 px-4 py-3 rounded-lg text-left">
                  <p className="font-semibold">Đang chờ phê duyệt yêu cầu tham gia nhóm</p>
                  <p className="text-sm mt-1">
                    Nhóm: {pendingJoinRequest.groupName || pendingJoinRequest.groupCode} • Hết hạn lúc{" "}
                    {new Date(pendingJoinRequest.expiresAt).toLocaleString("vi-VN")}
                  </p>
                </div>
              )}

              <div className="space-y-4 mb-6">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-2">Tên nhóm (tùy chọn)</label>
                  <input
                    type="text"
                    value={groupName}
                    onChange={(e) => setGroupName(e.target.value)}
                    placeholder="VD: Team UniThesis..."
                    maxLength={100}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                  />
                </div>
              </div>

              <button
                onClick={handleCreateGroup}
                disabled={creatingGroup || !!pendingJoinRequest}
                className="w-full bg-primary text-white px-6 py-3 rounded-lg font-semibold hover:bg-primary/90 disabled:opacity-50 transition"
              >
                {pendingJoinRequest ? "Đang chờ phê duyệt yêu cầu" : creatingGroup ? "Đang tạo..." : "✨ Tạo nhóm mới"}
              </button>
            </div>
          </div>
        ) : (
          <div className="max-w-4xl mx-auto space-y-6">
            {/* Group Info Card */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
              <div className="flex items-start justify-between mb-6">
                <div>
                  <h2 className="text-2xl font-bold text-gray-800 mb-1">{myGroup.groupName || myGroup.groupCode}</h2>
                  <p className="text-sm text-gray-500">Mã nhóm: {myGroup.groupCode}</p>
                  {myGroup.projectName && (
                    <p className="text-sm text-gray-600 mt-1">📋 Đề tài: {myGroup.projectName}</p>
                  )}
                </div>
                <span
                  className={`px-4 py-2 rounded-full text-sm font-semibold ${myGroup.isOpenForRequests ? "bg-green-100 text-green-800" : "bg-gray-100 text-gray-800"}`}
                >
                  {myGroup.isOpenForRequests ? "Mở" : "Kín"}
                </span>
              </div>

              <div className="grid grid-cols-2 gap-4 mb-6 p-4 bg-gray-50 rounded-lg">
                <div>
                  <p className="text-sm text-gray-600">Số thành viên</p>
                  <p className="text-2xl font-bold text-gray-800">
                    {myGroup.members?.length ?? 0}/{myGroup.maxMembers}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Ngày tạo</p>
                  <p className="text-lg font-semibold text-gray-800">
                    {new Date(myGroup.createdAt).toLocaleDateString("vi-VN")}
                  </p>
                </div>
              </div>

              <button
                onClick={() => setShowInviteModal(true)}
                className="w-full bg-blue-50 text-blue-700 px-4 py-3 rounded-lg font-semibold hover:bg-blue-100 transition"
              >
                ✉️ Mời thành viên mới
              </button>
            </div>

            {/* Members List */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
              <h3 className="text-lg font-bold text-gray-800 mb-4">Thành viên nhóm</h3>
              <div className="space-y-3">
                {myGroup.members && myGroup.members.length > 0 ? (
                  myGroup.members.map((member) => (
                    <div
                      key={member.studentId}
                      className="flex items-center justify-between p-3 border border-gray-200 rounded-lg hover:bg-gray-50"
                    >
                      <div>
                        <p className="font-semibold text-gray-800">{member.fullName}</p>
                        <p className="text-sm text-gray-500">
                          {member.studentCode} • {member.email}
                        </p>
                      </div>
                      <div className="text-right">
                        <p className="text-sm font-medium text-gray-600">{member.role}</p>
                        <p className="text-xs text-gray-500">
                          Tham gia: {new Date(member.joinedAt).toLocaleDateString("vi-VN")}
                        </p>
                      </div>
                    </div>
                  ))
                ) : (
                  <p className="text-gray-500 text-center py-4">Chưa có thành viên nào</p>
                )}
              </div>
            </div>

            {/* Join Requests */}
            {joinRequests.length > 0 && (
              <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
                <h3 className="text-lg font-bold text-gray-800 mb-4">Yêu cầu tham gia ({joinRequests.length})</h3>
                <div className="space-y-3">
                  {joinRequests.map((request) => (
                    <div key={request.id} className="p-4 border border-yellow-200 bg-yellow-50 rounded-lg">
                      <div className="flex items-start justify-between mb-3">
                        <div>
                          <p className="font-semibold text-gray-800">{request.studentName}</p>
                          <p className="text-sm text-gray-600">{request.studentCode}</p>
                          {request.message && <p className="text-sm text-gray-700 mt-2 italic">"{request.message}"</p>}
                        </div>
                        <span className="text-xs text-gray-500">
                          {new Date(request.createdAt).toLocaleDateString("vi-VN")}
                        </span>
                      </div>
                      <div className="flex gap-2 justify-end">
                        <button
                          onClick={() => handleRejectRequest(request.id)}
                          className="px-4 py-2 bg-red-50 text-red-700 rounded-lg font-semibold hover:bg-red-100 transition text-sm"
                        >
                          Từ chối
                        </button>
                        <button
                          onClick={() => handleApproveRequest(request.id)}
                          className="px-4 py-2 bg-green-50 text-green-700 rounded-lg font-semibold hover:bg-green-100 transition text-sm"
                        >
                          Chấp nhận
                        </button>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            )}
          </div>
        )}
      </main>

      {/* Invite Modal */}
      {showInviteModal && myGroup && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-lg shadow-lg max-w-md w-full p-6">
            <h3 className="text-xl font-bold text-gray-800 mb-4">Mời thành viên mới</h3>

            <div className="space-y-4 mb-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">Mã sinh viên</label>
                <input
                  type="text"
                  value={inviteData.studentCode}
                  onChange={(e) => setInviteData({ ...inviteData, studentCode: e.target.value })}
                  placeholder="Nhập mã sinh viên..."
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">Lời nhắn (tùy chọn)</label>
                <textarea
                  value={inviteData.message}
                  onChange={(e) => setInviteData({ ...inviteData, message: e.target.value })}
                  placeholder="Viết lời mời..."
                  maxLength={500}
                  rows={3}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary"
                />
              </div>
            </div>

            <div className="flex gap-3">
              <button
                onClick={() => setShowInviteModal(false)}
                className="flex-1 px-4 py-2 bg-gray-100 text-gray-700 rounded-lg font-semibold hover:bg-gray-200 transition"
              >
                Hủy
              </button>
              <button
                onClick={handleInviteMember}
                disabled={!inviteData.studentCode || inviting}
                className="flex-1 px-4 py-2 bg-primary text-white rounded-lg font-semibold hover:bg-primary/90 disabled:opacity-50 transition"
              >
                {inviting ? "Đang gửi..." : "Gửi lời mời"}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Success Modal */}
      <SuccessModal
        isOpen={showSuccessModal}
        onClose={() => setShowSuccessModal(false)}
        title="Mời thành viên thành công"
        message={successMessage}
        icon="person_add"
        autoClose={3000}
      />
    </div>
  );
}
